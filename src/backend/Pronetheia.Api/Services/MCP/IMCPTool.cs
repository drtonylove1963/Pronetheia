namespace Pronetheia.Api.Services.MCP;

public interface IMCPTool
{
    string Id { get; }
    string Name { get; }
    string Category { get; }
    string Description { get; }
    string SecurityLevel { get; }
    int ExecutionTimeout { get; }
    
    Task<ToolExecutionResult> Execute(Dictionary<string, object> parameters);
    Task<bool> ValidateParameters(Dictionary<string, object> parameters);
    Dictionary<string, object> GetInputSchema();
    Dictionary<string, object> GetOutputSchema();
}

public class ToolExecutionResult
{
    public bool Success { get; set; }
    public string ToolName { get; set; } = string.Empty;
    public object? Output { get; set; }
    public string? Error { get; set; }
    public int ExecutionTime { get; set; }
    public string SecurityLevel { get; set; } = "safe";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public interface IMCPToolRegistry
{
    Task RegisterTool(IMCPTool tool);
    Task<ToolExecutionResult> ExecuteTool(string toolName, Dictionary<string, object> parameters, string requestingAgent);
    Task<IMCPTool?> GetTool(string toolName);
    Task<List<ToolInfo>> GetAvailableTools();
    Task InitializeTools();
}

public class ToolInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SecurityLevel { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}