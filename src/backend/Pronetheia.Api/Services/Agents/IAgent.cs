using Pronetheia.Api.Models;

namespace Pronetheia.Api.Services.Agents;

public interface IAgent
{
    string Id { get; }
    string Name { get; }
    AgentType Type { get; }
    AgentStatus Status { get; }
    List<string> Capabilities { get; }
    
    Task<AgentResponse> ProcessMessage(AgentMessage message);
    Task<bool> CanHandleMessage(AgentMessage message);
    Task Initialize();
    Task Shutdown();
    Task<AgentHealthStatus> GetHealthStatus();
}

public class AgentMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FromAgent { get; set; } = string.Empty;
    public string ToAgent { get; set; } = string.Empty;
    public MessageType MessageType { get; set; }
    public object Content { get; set; } = new { };
    public string? UserId { get; set; }
    public bool RequiresResponse { get; set; } = true;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class AgentResponse
{
    public string AgentId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public object? Result { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class AgentHealthStatus
{
    public string AgentId { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public AgentStatus Status { get; set; }
    public DateTime LastActivity { get; set; }
    public Dictionary<string, object> Metrics { get; set; } = new();
}