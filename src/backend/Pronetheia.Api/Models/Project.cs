using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class Project
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<ProjectFile> Files { get; set; } = new List<ProjectFile>();
}