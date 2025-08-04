using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class AgentCommunication
{
    public int Id { get; set; }
    
    public int FromAgentId { get; set; }
    public virtual Agent FromAgent { get; set; } = null!;
    
    public int? ToAgentId { get; set; } // NULL for broadcast
    public virtual Agent? ToAgent { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string MessageType { get; set; } = string.Empty; // 'task', 'response', 'coordination', 'evolution'
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "pending"; // 'pending', 'processed', 'failed'
    
    public bool RequiresResponse { get; set; } = false;
    
    public int? ResponseToId { get; set; }
    public virtual AgentCommunication? ResponseTo { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public enum MessageType
{
    Task,
    Response,
    Coordination,
    Evolution
}

public enum CommunicationStatus
{
    Pending,
    Processed,
    Failed
}