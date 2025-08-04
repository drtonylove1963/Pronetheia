using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class MCPTool
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // 'file_ops', 'code_gen', 'analysis', 'database', 'web_search'
    
    public string? Description { get; set; }
    
    [Required]
    public string InputSchema { get; set; } = "{}"; // JSON schema
    
    [Required]
    public string OutputSchema { get; set; } = "{}"; // JSON schema
    
    [MaxLength(20)]
    public string SecurityLevel { get; set; } = "safe"; // 'safe', 'elevated', 'dangerous'
    
    public int ExecutionTimeout { get; set; } = 30000; // milliseconds
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<MCPToolExecution> Executions { get; set; } = new List<MCPToolExecution>();
}

public enum MCPCategory
{
    FileOperations,
    CodeGeneration,
    Analysis,
    Database,
    WebSearch
}

public enum SecurityLevel
{
    Safe,
    Elevated,
    Dangerous
}