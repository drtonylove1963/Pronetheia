using Pronetheia.Api.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Pronetheia.Api.Services.Agents;

public class ChatAgent : IAgent
{
    private readonly IOpenRouterService _openRouterService;
    private readonly IAgentCommunicationHub _communicationHub;
    private readonly ILogger _logger;
    
    public string Id => "chat-agent";
    public string Name => "ChatAgent";
    public AgentType Type => AgentType.Chat;
    public AgentStatus Status { get; private set; } = AgentStatus.Idle;
    public List<string> Capabilities => new()
    {
        "naturalLanguageProcessing",
        "conversationManagement",
        "userIntentRecognition",
        "agentCoordination",
        "contextRetention"
    };

    public ChatAgent(
        IOpenRouterService openRouterService,
        IAgentCommunicationHub communicationHub,
        ILogger<ChatAgent> logger)
    {
        _openRouterService = openRouterService;
        _communicationHub = communicationHub;
        _logger = logger;
    }

    public async Task<AgentResponse> ProcessMessage(AgentMessage message)
    {
        try
        {
            Status = AgentStatus.Busy;
            _logger.LogInformation("ChatAgent processing message: {MessageType}", message.MessageType);

            var response = message.MessageType switch
            {
                MessageType.Task => await HandleUserTask(message),
                MessageType.Coordination => await HandleCoordination(message),
                MessageType.Response => await HandleAgentResponse(message),
                _ => new AgentResponse
                {
                    AgentId = Id,
                    Success = false,
                    Error = $"Unsupported message type: {message.MessageType}"
                }
            };

            Status = AgentStatus.Idle;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message in ChatAgent");
            Status = AgentStatus.Error;
            return new AgentResponse
            {
                AgentId = Id,
                Success = false,
                Error = ex.Message
            };
        }
    }

    private async Task<AgentResponse> HandleUserTask(AgentMessage message)
    {
        var userMessage = message.Content?.ToString() ?? string.Empty;
        
        // Analyze user intent
        var intent = await AnalyzeUserIntent(userMessage);
        
        // Route to appropriate agent if needed
        if (intent.RequiresSpecializedAgent)
        {
            var routedResponse = await RouteToSpecializedAgent(intent, message);
            if (routedResponse != null)
                return routedResponse;
        }
        
        // Process with DeepSeek AI
        var aiResponse = await _openRouterService.SendMessage(userMessage);
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = true,
            Result = new
            {
                message = aiResponse,
                intent = intent,
                timestamp = DateTime.UtcNow
            },
            Metadata = new Dictionary<string, object>
            {
                ["conversationContext"] = message.UserId ?? "anonymous",
                ["processingTime"] = "0ms"
            }
        };
    }

    private async Task<AgentResponse> HandleCoordination(AgentMessage message)
    {
        // Handle coordination messages from other agents
        var coordinationData = JsonConvert.DeserializeObject<dynamic>(message.Content?.ToString() ?? "{}");
        
        _logger.LogInformation("Handling coordination from {FromAgent}", message.FromAgent);
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = true,
            Result = new
            {
                acknowledged = true,
                coordinationId = coordinationData?.coordinationId ?? Guid.NewGuid().ToString()
            }
        };
    }

    private async Task<AgentResponse> HandleAgentResponse(AgentMessage message)
    {
        // Process responses from other agents and format for user
        var agentResponse = JsonConvert.DeserializeObject<dynamic>(message.Content?.ToString() ?? "{}");
        
        var formattedResponse = await FormatAgentResponse(agentResponse, message.FromAgent);
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = true,
            Result = formattedResponse
        };
    }

    private async Task<UserIntent> AnalyzeUserIntent(string message)
    {
        // Simple intent analysis - can be enhanced with AI
        var intent = new UserIntent
        {
            RequiresSpecializedAgent = false,
            TargetAgent = null,
            IntentType = "general"
        };

        if (message.ToLower().Contains("evolve") || message.ToLower().Contains("improve") || 
            message.ToLower().Contains("analyze capability"))
        {
            intent.RequiresSpecializedAgent = true;
            intent.TargetAgent = "evolution-agent";
            intent.IntentType = "evolution";
        }
        else if (message.ToLower().Contains("execute tool") || message.ToLower().Contains("run mcp") ||
                 message.ToLower().Contains("file operation"))
        {
            intent.RequiresSpecializedAgent = true;
            intent.TargetAgent = "tool-agent";
            intent.IntentType = "tool-execution";
        }

        return await Task.FromResult(intent);
    }

    private async Task<AgentResponse?> RouteToSpecializedAgent(UserIntent intent, AgentMessage originalMessage)
    {
        if (string.IsNullOrEmpty(intent.TargetAgent))
            return null;

        var routedMessage = new AgentMessage
        {
            FromAgent = Id,
            ToAgent = intent.TargetAgent,
            MessageType = MessageType.Task,
            Content = originalMessage.Content,
            UserId = originalMessage.UserId,
            RequiresResponse = true
        };

        await _communicationHub.SendMessage(routedMessage);
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = true,
            Result = new
            {
                routed = true,
                targetAgent = intent.TargetAgent,
                message = $"Your request has been routed to {intent.TargetAgent} for specialized processing."
            }
        };
    }

    private async Task<object> FormatAgentResponse(dynamic agentResponse, string fromAgent)
    {
        return await Task.FromResult(new
        {
            source = fromAgent,
            response = agentResponse,
            formatted = true,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task<bool> CanHandleMessage(AgentMessage message)
    {
        // ChatAgent can handle user messages and coordination
        return await Task.FromResult(
            message.MessageType == MessageType.Task ||
            message.MessageType == MessageType.Coordination ||
            message.MessageType == MessageType.Response
        );
    }

    public async Task Initialize()
    {
        Status = AgentStatus.Active;
        _logger.LogInformation("ChatAgent initialized");
        await Task.CompletedTask;
    }

    public async Task Shutdown()
    {
        Status = AgentStatus.Idle;
        _logger.LogInformation("ChatAgent shutdown");
        await Task.CompletedTask;
    }

    public async Task<AgentHealthStatus> GetHealthStatus()
    {
        return await Task.FromResult(new AgentHealthStatus
        {
            AgentId = Id,
            IsHealthy = Status != AgentStatus.Error,
            Status = Status,
            LastActivity = DateTime.UtcNow,
            Metrics = new Dictionary<string, object>
            {
                ["messagesProcessed"] = 0,
                ["averageResponseTime"] = "0ms",
                ["activeConversations"] = 0
            }
        });
    }

    private class UserIntent
    {
        public bool RequiresSpecializedAgent { get; set; }
        public string? TargetAgent { get; set; }
        public string IntentType { get; set; } = "general";
    }
}