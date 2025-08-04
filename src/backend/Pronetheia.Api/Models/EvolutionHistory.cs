using System.ComponentModel.DataAnnotations;

namespace Pronetheia.Api.Models;

public class EvolutionHistory
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Version { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string EvolutionType { get; set; } = string.Empty; // 'agent_creation', 'capability_enhancement', 'system_optimization'
    
    [MaxLength(100)]
    public string? TargetAgent { get; set; } // Agent being evolved or created
    
    [Required]
    public string Changes { get; set; } = string.Empty; // JSON description of changes
    
    public string? GeneratedCode { get; set; } // Code generated during evolution
    
    public bool Success { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public int? ExecutionTime { get; set; } // milliseconds
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? Notes { get; set; }
}

public enum EvolutionType
{
    AgentCreation,
    CapabilityEnhancement,
    SystemOptimization
}