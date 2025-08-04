using Pronetheia.Api.Models;
using Pronetheia.Api.Services.MCP;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Pronetheia.Api.Services.Agents;

public class ToolAgent : IAgent
{
    private readonly IMCPToolRegistry _toolRegistry;
    private readonly IAgentCommunicationHub _communicationHub;
    private readonly ILogger _logger;
    
    public string Id => "tool-agent";
    public string Name => "ToolAgent";
    public AgentType Type => AgentType.Tool;
    public AgentStatus Status { get; private set; } = AgentStatus.Idle;
    public List<string> Capabilities => new()
    {
        "mcpToolOrchestration",
        "toolExecution",
        "resultProcessing",
        "securityValidation",
        "performanceMonitoring"
    };

    public ToolAgent(
        IMCPToolRegistry toolRegistry,
        IAgentCommunicationHub communicationHub,
        ILogger<ToolAgent> logger)
    {
        _toolRegistry = toolRegistry;
        _communicationHub = communicationHub;
        _logger = logger;
    }

    public async Task<AgentResponse> ProcessMessage(AgentMessage message)
    {
        try
        {
            Status = AgentStatus.Busy;
            _logger.LogInformation("ToolAgent processing message: {MessageType}", message.MessageType);

            var response = message.MessageType switch
            {
                MessageType.Task => await HandleToolExecution(message),
                MessageType.Coordination => await HandleCoordination(message),
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
            _logger.LogError(ex, "Error processing message in ToolAgent");
            Status = AgentStatus.Error;
            return new AgentResponse
            {
                AgentId = Id,
                Success = false,
                Error = ex.Message
            };
        }
    }

    private async Task<AgentResponse> HandleToolExecution(AgentMessage message)
    {
        var executionRequest = ParseExecutionRequest(message.Content);
        
        if (executionRequest == null)
        {
            return new AgentResponse
            {
                AgentId = Id,
                Success = false,
                Error = "Invalid tool execution request format"
            };
        }
        
        _logger.LogInformation("Executing MCP tool: {ToolName}", executionRequest.ToolName);
        
        // Validate security level
        var securityCheck = await ValidateToolSecurity(executionRequest);
        if (!securityCheck.IsValid)
        {
            return new AgentResponse
            {
                AgentId = Id,
                Success = false,
                Error = $"Security validation failed: {securityCheck.Reason}"
            };
        }
        
        // Execute the tool
        var executionResult = await _toolRegistry.ExecuteTool(
            executionRequest.ToolName,
            executionRequest.Parameters,
            Id
        );
        
        // Process and format the result
        var formattedResult = await ProcessToolResult(executionResult);
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = executionResult.Success,
            Result = formattedResult,
            Error = executionResult.Error,
            Metadata = new Dictionary<string, object>
            {
                ["toolName"] = executionRequest.ToolName,
                ["executionTime"] = executionResult.ExecutionTime,
                ["securityLevel"] = executionResult.SecurityLevel
            }
        };
    }

    private async Task<AgentResponse> HandleCoordination(AgentMessage message)
    {
        var coordinationData = JsonConvert.DeserializeObject<dynamic>(message.Content?.ToString() ?? "{}");
        
        _logger.LogInformation("ToolAgent coordinating with {FromAgent}", message.FromAgent);
        
        // If coordination involves tool discovery
        if (coordinationData?.action == "discover")
        {
            var availableTools = await _toolRegistry.GetAvailableTools();
            return new AgentResponse
            {
                AgentId = Id,
                Success = true,
                Result = new
                {
                    availableTools = availableTools,
                    totalTools = availableTools.Count
                }
            };
        }
        
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

    private ToolExecutionRequest? ParseExecutionRequest(object? content)
    {
        try
        {
            if (content == null) return null;
            
            var json = content.ToString();
            if (string.IsNullOrEmpty(json)) return null;
            
            // Try to parse as structured request
            try
            {
                return JsonConvert.DeserializeObject<ToolExecutionRequest>(json);
            }
            catch
            {
                // Try to parse as simple string command
                return ParseStringCommand(json);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse tool execution request");
            return null;
        }
    }

    private ToolExecutionRequest ParseStringCommand(string command)
    {
        // Parse simple commands like "file read /path/to/file"
        var parts = command.Split(' ', 3);
        if (parts.Length < 2)
        {
            return new ToolExecutionRequest
            {
                ToolName = "unknown",
                Parameters = new Dictionary<string, object> { ["command"] = command }
            };
        }
        
        var toolCategory = parts[0].ToLower();
        var action = parts[1].ToLower();
        var param = parts.Length > 2 ? parts[2] : "";
        
        var toolName = toolCategory switch
        {
            "file" => "FileOperationsMCP",
            "code" => "CodeGenerationMCP",
            "analyze" => "AnalysisMCP",
            "database" or "db" => "DatabaseMCP",
            "web" or "search" => "WebSearchMCP",
            _ => "unknown"
        };
        
        return new ToolExecutionRequest
        {
            ToolName = toolName,
            Parameters = new Dictionary<string, object>
            {
                ["action"] = action,
                ["target"] = param
            }
        };
    }

    private async Task<SecurityValidation> ValidateToolSecurity(ToolExecutionRequest request)
    {
        // Implement security validation logic
        var tool = await _toolRegistry.GetTool(request.ToolName);
        if (tool == null)
        {
            return new SecurityValidation
            {
                IsValid = false,
                Reason = "Tool not found"
            };
        }
        
        // Check if tool requires elevated permissions
        if (tool.SecurityLevel == "dangerous")
        {
            // Would require additional validation in production
            _logger.LogWarning("Executing dangerous tool: {ToolName}", request.ToolName);
        }
        
        return new SecurityValidation
        {
            IsValid = true,
            Reason = null
        };
    }

    private async Task<object> ProcessToolResult(ToolExecutionResult result)
    {
        // Format the result based on tool type
        var formatted = new
        {
            success = result.Success,
            toolName = result.ToolName,
            output = result.Output,
            error = result.Error,
            metadata = new
            {
                executionTime = $"{result.ExecutionTime}ms",
                timestamp = result.Timestamp,
                securityLevel = result.SecurityLevel
            }
        };
        
        return await Task.FromResult(formatted);
    }

    public async Task<bool> CanHandleMessage(AgentMessage message)
    {
        return await Task.FromResult(
            message.MessageType == MessageType.Task ||
            message.MessageType == MessageType.Coordination
        );
    }

    public async Task Initialize()
    {
        Status = AgentStatus.Active;
        _logger.LogInformation("ToolAgent initialized");
        
        // Register core MCP tools
        await _toolRegistry.InitializeTools();
        
        var tools = await _toolRegistry.GetAvailableTools();
        _logger.LogInformation("ToolAgent loaded {ToolCount} MCP tools", tools.Count);
    }

    public async Task Shutdown()
    {
        Status = AgentStatus.Idle;
        _logger.LogInformation("ToolAgent shutdown");
        await Task.CompletedTask;
    }

    public async Task<AgentHealthStatus> GetHealthStatus()
    {
        var availableTools = await _toolRegistry.GetAvailableTools();
        
        return new AgentHealthStatus
        {
            AgentId = Id,
            IsHealthy = Status != AgentStatus.Error,
            Status = Status,
            LastActivity = DateTime.UtcNow,
            Metrics = new Dictionary<string, object>
            {
                ["toolsAvailable"] = availableTools.Count,
                ["executionsCompleted"] = 0,
                ["averageExecutionTime"] = "0ms"
            }
        };
    }

    private class ToolExecutionRequest
    {
        public string ToolName { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public string? RequestingAgent { get; set; }
        public int Priority { get; set; } = 1;
    }

    private class SecurityValidation
    {
        public bool IsValid { get; set; }
        public string? Reason { get; set; }
    }
}