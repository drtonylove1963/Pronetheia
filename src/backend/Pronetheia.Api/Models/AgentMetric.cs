using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class AgentMetric
{
    public int Id { get; set; }
    
    public int AgentId { get; set; }
    public virtual Agent Agent { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string MetricType { get; set; } = string.Empty; // 'response_time', 'success_rate', 'task_completion'
    
    public decimal MetricValue { get; set; }
    
    [MaxLength(20)]
    public string? Unit { get; set; } // 'ms', 'percent', 'count'
    
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}

public enum MetricType
{
    ResponseTime,
    SuccessRate,
    TaskCompletion
}