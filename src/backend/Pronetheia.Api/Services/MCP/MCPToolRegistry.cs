using System.Diagnostics;
using Pronetheia.Api.Data;
using Pronetheia.Api.Models;
using Microsoft.Extensions.Logging;

namespace Pronetheia.Api.Services.MCP;

public class MCPToolRegistry : IMCPToolRegistry
{
    private readonly Dictionary<string, IMCPTool> _tools = new();
    private readonly PronetheiaDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly object _lockObject = new();

    public MCPToolRegistry(PronetheiaDbContext dbContext, ILogger<MCPToolRegistry> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task RegisterTool(IMCPTool tool)
    {
        lock (_lockObject)
        {
            _tools[tool.Name] = tool;
        }
        
        _logger.LogInformation("Registered MCP tool: {ToolName} ({Category})", tool.Name, tool.Category);
        
        // Update database (if available)
        try
        {
            var dbTool = _dbContext.MCPTools.FirstOrDefault(t => t.Name == tool.Name);
            if (dbTool == null)
            {
                dbTool = new MCPTool
                {
                    Name = tool.Name,
                    Category = tool.Category,
                    Description = tool.Description,
                    SecurityLevel = tool.SecurityLevel,
                    ExecutionTimeout = tool.ExecutionTimeout,
                    InputSchema = System.Text.Json.JsonSerializer.Serialize(tool.GetInputSchema()),
                    OutputSchema = System.Text.Json.JsonSerializer.Serialize(tool.GetOutputSchema()),
                    IsActive = true
                };
                _dbContext.MCPTools.Add(dbTool);
            }
            else
            {
                dbTool.IsActive = true;
            }
            
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not save tool {ToolName} to database, continuing in memory only", tool.Name);
        }
    }

    public async Task<ToolExecutionResult> ExecuteTool(
        string toolName, 
        Dictionary<string, object> parameters, 
        string requestingAgent)
    {
        if (!_tools.ContainsKey(toolName))
        {
            _logger.LogWarning("Tool not found: {ToolName}", toolName);
            return new ToolExecutionResult
            {
                Success = false,
                ToolName = toolName,
                Error = $"Tool '{toolName}' not found in registry"
            };
        }

        var tool = _tools[toolName];
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Executing tool {ToolName} for agent {Agent}", toolName, requestingAgent);
            
            // Validate parameters
            var isValid = await tool.ValidateParameters(parameters);
            if (!isValid)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    ToolName = toolName,
                    Error = "Invalid parameters provided"
                };
            }
            
            // Execute with timeout
            var cts = new CancellationTokenSource(tool.ExecutionTimeout);
            var executionTask = tool.Execute(parameters);
            var timeoutTask = Task.Delay(tool.ExecutionTimeout, cts.Token);
            
            var completedTask = await Task.WhenAny(executionTask, timeoutTask);
            
            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("Tool execution timeout: {ToolName}", toolName);
                return new ToolExecutionResult
                {
                    Success = false,
                    ToolName = toolName,
                    Error = $"Execution timeout ({tool.ExecutionTimeout}ms)",
                    ExecutionTime = (int)stopwatch.ElapsedMilliseconds
                };
            }
            
            cts.Cancel();
            var result = await executionTask;
            result.ExecutionTime = (int)stopwatch.ElapsedMilliseconds;
            
            // Log execution to database
            await LogExecution(toolName, requestingAgent, parameters, result);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool {ToolName}", toolName);
            return new ToolExecutionResult
            {
                Success = false,
                ToolName = toolName,
                Error = ex.Message,
                ExecutionTime = (int)stopwatch.ElapsedMilliseconds
            };
        }
    }

    public async Task<IMCPTool?> GetTool(string toolName)
    {
        return await Task.FromResult(_tools.ContainsKey(toolName) ? _tools[toolName] : null);
    }

    public async Task<List<ToolInfo>> GetAvailableTools()
    {
        var tools = _tools.Values.Select(t => new ToolInfo
        {
            Id = t.Id,
            Name = t.Name,
            Category = t.Category,
            Description = t.Description,
            SecurityLevel = t.SecurityLevel,
            IsActive = true
        }).ToList();
        
        return await Task.FromResult(tools);
    }

    public async Task InitializeTools()
    {
        _logger.LogInformation("Initializing MCP tools from database");
        
        try
        {
            var dbTools = _dbContext.MCPTools.Where(t => t.IsActive).ToList();
            _logger.LogInformation("Found {Count} active tools in database", dbTools.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load tools from database, continuing with default configuration");
            // Continue without database tools - they will be registered via dependency injection
        }
        
        // Tools will be registered by dependency injection
        await Task.CompletedTask;
    }

    private async Task LogExecution(
        string toolName, 
        string requestingAgent, 
        Dictionary<string, object> parameters, 
        ToolExecutionResult result)
    {
        try
        {
            var tool = _dbContext.MCPTools.FirstOrDefault(t => t.Name == toolName);
            var agent = _dbContext.Agents.FirstOrDefault(a => a.Name == requestingAgent || a.Type == requestingAgent);
            
            if (tool != null && agent != null)
            {
                var execution = new MCPToolExecution
                {
                    ToolId = tool.Id,
                    RequestingAgentId = agent.Id,
                    InputParameters = System.Text.Json.JsonSerializer.Serialize(parameters),
                    OutputResult = result.Output != null ? System.Text.Json.JsonSerializer.Serialize(result.Output) : null,
                    Status = result.Success ? "completed" : "failed",
                    ExecutionTime = result.ExecutionTime,
                    ErrorMessage = result.Error,
                    StartedAt = result.Timestamp,
                    CompletedAt = DateTime.UtcNow
                };
                
                _dbContext.MCPToolExecutions.Add(execution);
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log tool execution");
        }
    }
}