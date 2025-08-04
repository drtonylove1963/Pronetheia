using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class CapabilityGap
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Area { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    public int Priority { get; set; } = 0;
    
    [MaxLength(20)]
    public string Status { get; set; } = "identified"; // 'identified', 'planned', 'in_progress', 'completed'
    
    public string? ProposedSolution { get; set; }
    
    public string? RequiredCapabilities { get; set; } // JSON array
    
    [MaxLength(50)]
    public string TargetPlatform { get; set; } = "web";
    
    public DateTime IdentifiedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum GapStatus
{
    Identified,
    Planned,
    InProgress,
    Completed
}