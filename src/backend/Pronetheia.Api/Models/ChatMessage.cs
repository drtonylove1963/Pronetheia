using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class ChatMessage
{
    public int Id { get; set; }
    
    public int SessionId { get; set; }
    public virtual ChatSession Session { get; set; } = null!;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = string.Empty; // 'user', 'assistant', 'system'
    
    public int? AgentId { get; set; } // Which agent responded
    public virtual Agent? Agent { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? Metadata { get; set; } // JSON for attachments, etc.
}

public enum MessageRole
{
    User,
    Assistant,
    System
}