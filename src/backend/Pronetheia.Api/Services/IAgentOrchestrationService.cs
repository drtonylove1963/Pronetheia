using Pronetheia.Api.Services.Agents;
using Pronetheia.Api.Services.MCP;
using Pronetheia.Api.Models;

namespace Pronetheia.Api.Services;

public interface IAgentOrchestrationService
{
    Task InitializeAgents();
    Task<AgentResponse> RouteUserMessage(string message, string userId);
    Task<List<AgentHealthStatus>> GetAgentStatuses();
    Task<AgentResponse> ExecuteAgentTask(string agentId, object task);
}

public class AgentOrchestrationService : IAgentOrchestrationService
{
    private readonly IAgentCommunicationHub _communicationHub;
    private readonly IMCPToolRegistry _toolRegistry;
    private readonly ChatAgent _chatAgent;
    private readonly EvolutionAgent _evolutionAgent;
    private readonly ToolAgent _toolAgent;
    private readonly ProjectManagementAgent _projectManagementAgent;
    private readonly IServiceProvider _serviceProvider;

    public AgentOrchestrationService(
        IAgentCommunicationHub communicationHub,
        IMCPToolRegistry toolRegistry,
        ChatAgent chatAgent,
        EvolutionAgent evolutionAgent,
        ToolAgent toolAgent,
        ProjectManagementAgent projectManagementAgent,
        IServiceProvider serviceProvider)
    {
        _communicationHub = communicationHub;
        _toolRegistry = toolRegistry;
        _chatAgent = chatAgent;
        _evolutionAgent = evolutionAgent;
        _toolAgent = toolAgent;
        _projectManagementAgent = projectManagementAgent;
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAgents()
    {
        // Register core agents
        await _communicationHub.RegisterAgent(_chatAgent);
        await _communicationHub.RegisterAgent(_evolutionAgent);
        await _communicationHub.RegisterAgent(_toolAgent);
        await _communicationHub.RegisterAgent(_projectManagementAgent); // ✨ First Evolved Agent
        
        // Register MCP tools
        var fileOps = _serviceProvider.GetService(typeof(FileOperationsMCP)) as FileOperationsMCP;
        var codeGen = _serviceProvider.GetService(typeof(CodeGenerationMCP)) as CodeGenerationMCP;
        var analysis = _serviceProvider.GetService(typeof(AnalysisMCP)) as AnalysisMCP;
        var database = _serviceProvider.GetService(typeof(DatabaseMCP)) as DatabaseMCP;
        var webSearch = _serviceProvider.GetService(typeof(WebSearchMCP)) as WebSearchMCP;
        
        if (fileOps != null) await _toolRegistry.RegisterTool(fileOps);
        if (codeGen != null) await _toolRegistry.RegisterTool(codeGen);
        if (analysis != null) await _toolRegistry.RegisterTool(analysis);
        if (database != null) await _toolRegistry.RegisterTool(database);
        if (webSearch != null) await _toolRegistry.RegisterTool(webSearch);
        
        // Start coordination
        await _communicationHub.StartCoordination();
    }

    public async Task<AgentResponse> RouteUserMessage(string message, string userId)
    {
        // Route through ChatAgent first for intent analysis
        var agentMessage = new AgentMessage
        {
            FromAgent = "user",
            ToAgent = _chatAgent.Id,
            MessageType = MessageType.Task,
            Content = message,
            UserId = userId,
            RequiresResponse = true
        };
        
        var response = await _communicationHub.SendAndWaitForResponse(agentMessage, TimeSpan.FromSeconds(30));
        
        return response ?? new AgentResponse
        {
            AgentId = "system",
            Success = false,
            Error = "No response received from agent"
        };
    }

    public async Task<List<AgentHealthStatus>> GetAgentStatuses()
    {
        var agents = await _communicationHub.GetActiveAgents();
        var statuses = new List<AgentHealthStatus>();
        
        foreach (var agent in agents)
        {
            statuses.Add(await agent.GetHealthStatus());
        }
        
        return statuses;
    }

    public async Task<AgentResponse> ExecuteAgentTask(string agentId, object task)
    {
        var message = new AgentMessage
        {
            FromAgent = "system",
            ToAgent = agentId,
            MessageType = MessageType.Task,
            Content = task,
            RequiresResponse = true
        };
        
        var response = await _communicationHub.SendAndWaitForResponse(message, TimeSpan.FromSeconds(60));
        
        return response ?? new AgentResponse
        {
            AgentId = agentId,
            Success = false,
            Error = "Task execution failed"
        };
    }
}