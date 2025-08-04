using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class Agent
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty; // 'chat', 'evolution', 'tool'
    
    [MaxLength(20)]
    public string Status { get; set; } = "idle"; // 'active', 'idle', 'busy', 'error'
    
    public string Capabilities { get; set; } = "[]"; // JSON array
    
    public string? Configuration { get; set; } // JSON configuration
    
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<AgentCommunication> SentCommunications { get; set; } = new List<AgentCommunication>();
    public virtual ICollection<AgentCommunication> ReceivedCommunications { get; set; } = new List<AgentCommunication>();
    public virtual ICollection<MCPToolExecution> ToolExecutions { get; set; } = new List<MCPToolExecution>();
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<AgentMetric> Metrics { get; set; } = new List<AgentMetric>();
}

public enum AgentType
{
    Chat,
    Evolution,
    Tool
}

public enum AgentStatus
{
    Active,
    Idle,
    Busy,
    Error
}