using Pronetheia.Api.Services.Agents;
using Pronetheia.Api.Models;

namespace Pronetheia.Api.Services;

public interface IAgentCommunicationHub
{
    Task SendMessage(AgentMessage message);
    Task BroadcastMessage(AgentMessage message);
    Task<AgentResponse?> SendAndWaitForResponse(AgentMessage message, TimeSpan timeout);
    Task RegisterAgent(IAgent agent);
    Task UnregisterAgent(string agentId);
    Task<List<IAgent>> GetActiveAgents();
    Task StartCoordination();
    Task StopCoordination();
}

public class AgentCommunicationHub : IAgentCommunicationHub
{
    private readonly Dictionary<string, IAgent> _agents = new();
    private readonly Queue<AgentMessage> _messageQueue = new();
    private readonly object _lockObject = new();
    private bool _isRunning = false;
    private Task? _processingTask;

    public async Task SendMessage(AgentMessage message)
    {
        lock (_lockObject)
        {
            _messageQueue.Enqueue(message);
        }
        await Task.CompletedTask;
    }

    public async Task BroadcastMessage(AgentMessage message)
    {
        foreach (var agent in _agents.Values)
        {
            var broadcastMessage = new AgentMessage
            {
                FromAgent = message.FromAgent,
                ToAgent = agent.Id,
                MessageType = message.MessageType,
                Content = message.Content,
                UserId = message.UserId,
                RequiresResponse = false
            };
            await SendMessage(broadcastMessage);
        }
    }

    public async Task<AgentResponse?> SendAndWaitForResponse(AgentMessage message, TimeSpan timeout)
    {
        await SendMessage(message);
        
        // Simplified - in production would implement proper async waiting
        await Task.Delay(100);
        
        if (_agents.ContainsKey(message.ToAgent))
        {
            return await _agents[message.ToAgent].ProcessMessage(message);
        }
        
        return null;
    }

    public async Task RegisterAgent(IAgent agent)
    {
        lock (_lockObject)
        {
            _agents[agent.Id] = agent;
        }
        await agent.Initialize();
    }

    public async Task UnregisterAgent(string agentId)
    {
        if (_agents.ContainsKey(agentId))
        {
            await _agents[agentId].Shutdown();
            lock (_lockObject)
            {
                _agents.Remove(agentId);
            }
        }
    }

    public async Task<List<IAgent>> GetActiveAgents()
    {
        return await Task.FromResult(_agents.Values.ToList());
    }

    public async Task StartCoordination()
    {
        _isRunning = true;
        _processingTask = Task.Run(ProcessMessages);
        await Task.CompletedTask;
    }

    public async Task StopCoordination()
    {
        _isRunning = false;
        if (_processingTask != null)
        {
            await _processingTask;
        }
    }

    private async Task ProcessMessages()
    {
        while (_isRunning)
        {
            AgentMessage? message = null;
            
            lock (_lockObject)
            {
                if (_messageQueue.Count > 0)
                {
                    message = _messageQueue.Dequeue();
                }
            }
            
            if (message != null && _agents.ContainsKey(message.ToAgent))
            {
                try
                {
                    var response = await _agents[message.ToAgent].ProcessMessage(message);
                    
                    if (message.RequiresResponse && !string.IsNullOrEmpty(message.FromAgent))
                    {
                        var responseMessage = new AgentMessage
                        {
                            FromAgent = message.ToAgent,
                            ToAgent = message.FromAgent,
                            MessageType = MessageType.Response,
                            Content = response,
                            RequiresResponse = false
                        };
                        await SendMessage(responseMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            }
            
            await Task.Delay(10);
        }
    }
}