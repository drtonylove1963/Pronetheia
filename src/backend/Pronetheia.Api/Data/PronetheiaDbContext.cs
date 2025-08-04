using Microsoft.EntityFrameworkCore;
using Pronetheia.Api.Models;

namespace Pronetheia.Api.Data;

public class PronetheiaDbContext : DbContext
{
    public PronetheiaDbContext(DbContextOptions<PronetheiaDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<AgentCommunication> AgentCommunications { get; set; }
    public DbSet<MCPTool> MCPTools { get; set; }
    public DbSet<MCPToolExecution> MCPToolExecutions { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectFile> ProjectFiles { get; set; }
    public DbSet<EvolutionHistory> EvolutionHistories { get; set; }
    public DbSet<CapabilityGap> CapabilityGaps { get; set; }
    public DbSet<AgentMetric> AgentMetrics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Agent entity configuration
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.Property(e => e.Status).HasDefaultValue("idle");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.LastActivity).HasDefaultValueSql("GETUTCDATE()");
        });

        // AgentCommunication entity configuration
        modelBuilder.Entity<AgentCommunication>(entity =>
        {
            entity.HasOne(e => e.FromAgent)
                .WithMany(a => a.SentCommunications)
                .HasForeignKey(e => e.FromAgentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ToAgent)
                .WithMany(a => a.ReceivedCommunications)
                .HasForeignKey(e => e.ToAgentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ResponseTo)
                .WithMany()
                .HasForeignKey(e => e.ResponseToId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Status).HasDefaultValue("pending");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");
        });

        // MCPTool entity configuration
        modelBuilder.Entity<MCPTool>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.SecurityLevel).HasDefaultValue("safe");
            entity.Property(e => e.ExecutionTimeout).HasDefaultValue(30000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // MCPToolExecution entity configuration
        modelBuilder.Entity<MCPToolExecution>(entity =>
        {
            entity.HasOne(e => e.Tool)
                .WithMany(t => t.Executions)
                .HasForeignKey(e => e.ToolId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RequestingAgent)
                .WithMany(a => a.ToolExecutions)
                .HasForeignKey(e => e.RequestingAgentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Status).HasDefaultValue("pending");
            entity.Property(e => e.StartedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // ChatSession entity configuration
        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany(u => u.ChatSessions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // ChatMessage entity configuration
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasOne(e => e.Session)
                .WithMany(s => s.Messages)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Agent)
                .WithMany(a => a.ChatMessages)
                .HasForeignKey(e => e.AgentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(e => e.Role).HasConversion<string>();
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");
        });

        // Project entity configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // ProjectFile entity configuration
        modelBuilder.Entity<ProjectFile>(entity =>
        {
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Files)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // EvolutionHistory entity configuration
        modelBuilder.Entity<EvolutionHistory>(entity =>
        {
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");
        });

        // CapabilityGap entity configuration
        modelBuilder.Entity<CapabilityGap>(entity =>
        {
            entity.Property(e => e.Status).HasDefaultValue("identified");
            entity.Property(e => e.Priority).HasDefaultValue(0);
            entity.Property(e => e.TargetPlatform).HasDefaultValue("web");
            entity.Property(e => e.IdentifiedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // AgentMetric entity configuration
        modelBuilder.Entity<AgentMetric>(entity =>
        {
            entity.HasOne(e => e.Agent)
                .WithMany(a => a.Metrics)
                .HasForeignKey(e => e.AgentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.MetricValue).HasPrecision(10, 4);
            entity.Property(e => e.RecordedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed initial agents
        modelBuilder.Entity<Agent>().HasData(
            new Agent
            {
                Id = 1,
                Name = "ChatAgent",
                Type = "chat",
                Status = "idle",
                Capabilities = "[\"naturalLanguageProcessing\",\"conversationManagement\",\"userIntentRecognition\",\"agentCoordination\",\"contextRetention\"]",
                IsActive = true
            },
            new Agent
            {
                Id = 2,
                Name = "EvolutionAgent",
                Type = "evolution",
                Status = "idle",
                Capabilities = "[\"selfAnalysis\",\"capabilityGapIdentification\",\"agentGeneration\",\"codeGeneration\",\"systemOptimization\"]",
                IsActive = true
            },
            new Agent
            {
                Id = 3,
                Name = "ToolAgent",
                Type = "tool",
                Status = "idle",
                Capabilities = "[\"mcpToolOrchestration\",\"toolExecution\",\"resultProcessing\",\"securityValidation\",\"performanceMonitoring\"]",
                IsActive = true
            }
        );

        // Seed initial MCP tools
        modelBuilder.Entity<MCPTool>().HasData(
            new MCPTool
            {
                Id = 1,
                Name = "FileOperationsMCP",
                Category = "file_ops",
                Description = "File system operations including create, read, update, delete, and directory listing",
                InputSchema = "{\"type\":\"object\",\"properties\":{\"operation\":{\"type\":\"string\",\"enum\":[\"create\",\"read\",\"update\",\"delete\",\"list\"]},\"path\":{\"type\":\"string\"},\"content\":{\"type\":\"string\"}}}",
                OutputSchema = "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"result\":{\"type\":\"any\"},\"error\":{\"type\":\"string\"}}}",
                SecurityLevel = "elevated",
                ExecutionTimeout = 10000,
                IsActive = true
            },
            new MCPTool
            {
                Id = 2,
                Name = "CodeGenerationMCP",
                Category = "code_gen",
                Description = "AI-powered code generation, analysis, refactoring, test generation, and syntax validation",
                InputSchema = "{\"type\":\"object\",\"properties\":{\"action\":{\"type\":\"string\",\"enum\":[\"generate\",\"analyze\",\"refactor\",\"test\",\"validate\"]},\"prompt\":{\"type\":\"string\"},\"language\":{\"type\":\"string\"},\"code\":{\"type\":\"string\"}}}",
                OutputSchema = "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"code\":{\"type\":\"string\"},\"analysis\":{\"type\":\"object\"},\"error\":{\"type\":\"string\"}}}",
                SecurityLevel = "safe",
                ExecutionTimeout = 60000,
                IsActive = true
            },
            new MCPTool
            {
                Id = 3,
                Name = "AnalysisMCP",
                Category = "analysis",
                Description = "Codebase analysis including pattern identification, capability assessment, dependency analysis, and metrics calculation",
                InputSchema = "{\"type\":\"object\",\"properties\":{\"analysisType\":{\"type\":\"string\",\"enum\":[\"codebase\",\"patterns\",\"capabilities\",\"dependencies\",\"metrics\"]},\"targetPath\":{\"type\":\"string\"},\"options\":{\"type\":\"object\"}}}",
                OutputSchema = "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"analysis\":{\"type\":\"object\"},\"error\":{\"type\":\"string\"}}}",
                SecurityLevel = "safe",
                ExecutionTimeout = 30000,
                IsActive = true
            },
            new MCPTool
            {
                Id = 4,
                Name = "DatabaseMCP",
                Category = "database",
                Description = "Database operations including query execution, schema management, and CRUD operations",
                InputSchema = "{\"type\":\"object\",\"properties\":{\"operation\":{\"type\":\"string\",\"enum\":[\"query\",\"create\",\"insert\",\"update\",\"delete\"]},\"query\":{\"type\":\"string\"},\"table\":{\"type\":\"string\"},\"data\":{\"type\":\"object\"}}}",
                OutputSchema = "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"result\":{\"type\":\"any\"},\"rowsAffected\":{\"type\":\"number\"},\"error\":{\"type\":\"string\"}}}",
                SecurityLevel = "elevated",
                ExecutionTimeout = 15000,
                IsActive = true
            },
            new MCPTool
            {
                Id = 5,
                Name = "WebSearchMCP",
                Category = "web_search",
                Description = "Web search and content analysis including search, content fetching, website analysis, and documentation lookup",
                InputSchema = "{\"type\":\"object\",\"properties\":{\"action\":{\"type\":\"string\",\"enum\":[\"search\",\"fetch\",\"analyze\",\"documentation\",\"trends\"]},\"query\":{\"type\":\"string\"},\"url\":{\"type\":\"string\"},\"options\":{\"type\":\"object\"}}}",
                OutputSchema = "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"results\":{\"type\":\"array\"},\"content\":{\"type\":\"string\"},\"analysis\":{\"type\":\"object\"},\"error\":{\"type\":\"string\"}}}",
                SecurityLevel = "safe",
                ExecutionTimeout = 20000,
                IsActive = true
            }
        );
    }
}