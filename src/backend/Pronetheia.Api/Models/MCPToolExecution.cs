using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class MCPToolExecution
{
    public int Id { get; set; }
    
    public int ToolId { get; set; }
    public virtual MCPTool Tool { get; set; } = null!;
    
    public int RequestingAgentId { get; set; }
    public virtual Agent RequestingAgent { get; set; } = null!;
    
    [Required]
    public string InputParameters { get; set; } = "{}"; // JSON
    
    public string? OutputResult { get; set; } // JSON result
    
    [MaxLength(20)]
    public string Status { get; set; } = "pending"; // 'pending', 'executing', 'completed', 'failed'
    
    public int? ExecutionTime { get; set; } // milliseconds
    
    public string? ErrorMessage { get; set; }
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
}

public enum ExecutionStatus
{
    Pending,
    Executing,
    Completed,
    Failed
}