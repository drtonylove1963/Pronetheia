using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class ProjectFile
{
    public int Id { get; set; }
    
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;
    
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
    
    public string? Content { get; set; }
    
    [MaxLength(50)]
    public string? FileType { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}