using Pronetheia.Api.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Pronetheia.Api.Services.MCP;

public class DatabaseMCP : IMCPTool
{
    private readonly PronetheiaDbContext _dbContext;
    private readonly ILogger _logger;

    public string Id => "database";
    public string Name => "DatabaseMCP";
    public string Category => "database";
    public string Description => "Database operations including query execution, schema management, and CRUD operations";
    public string SecurityLevel => "elevated";
    public int ExecutionTimeout => 15000;

    public DatabaseMCP(PronetheiaDbContext dbContext, ILogger<DatabaseMCP> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ToolExecutionResult> Execute(Dictionary<string, object> parameters)
    {
        try
        {
            var operation = parameters.GetValueOrDefault("operation")?.ToString() ?? "";
            
            object? result = operation.ToLower() switch
            {
                "query" => await ExecuteQuery(parameters),
                "insert" => await InsertRecord(parameters),
                "update" => await UpdateRecord(parameters),
                "delete" => await DeleteRecord(parameters),
                _ => throw new ArgumentException($"Unknown operation: {operation}")
            };

            return new ToolExecutionResult
            {
                Success = true,
                ToolName = Name,
                Output = result,
                SecurityLevel = SecurityLevel
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database operation failed");
            return new ToolExecutionResult
            {
                Success = false,
                ToolName = Name,
                Error = ex.Message,
                SecurityLevel = SecurityLevel
            };
        }
    }

    private async Task<object> ExecuteQuery(Dictionary<string, object> parameters)
    {
        // Simplified - only allow safe queries on specific tables
        var table = parameters.GetValueOrDefault("table")?.ToString() ?? "";
        
        var result = table.ToLower() switch
        {
            "agents" => (object)await _dbContext.Agents.Select(a => new { a.Id, a.Name, a.Type, a.Status }).ToListAsync(),
            "mcptools" => (object)await _dbContext.MCPTools.Select(t => new { t.Id, t.Name, t.Category }).ToListAsync(),
            _ => throw new ArgumentException($"Access to table '{table}' is not allowed")
        };
        
        return new { table, records = result, count = GetResultCount(result) };
    }

    private int GetResultCount(object result)
    {
        if (result is System.Collections.ICollection collection)
        {
            return collection.Count;
        }
        return 0;
    }

    private async Task<object> InsertRecord(Dictionary<string, object> parameters)
    {
        // Simplified implementation
        return await Task.FromResult(new { inserted = false, message = "Insert operations are restricted" });
    }

    private async Task<object> UpdateRecord(Dictionary<string, object> parameters)
    {
        // Simplified implementation
        return await Task.FromResult(new { updated = false, message = "Update operations are restricted" });
    }

    private async Task<object> DeleteRecord(Dictionary<string, object> parameters)
    {
        // Simplified implementation
        return await Task.FromResult(new { deleted = false, message = "Delete operations are restricted" });
    }

    public async Task<bool> ValidateParameters(Dictionary<string, object> parameters)
    {
        var operation = parameters.GetValueOrDefault("operation")?.ToString();
        return await Task.FromResult(!string.IsNullOrEmpty(operation));
    }

    public Dictionary<string, object> GetInputSchema()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = new Dictionary<string, object>
            {
                ["operation"] = new Dictionary<string, object>
                {
                    ["type"] = "string",
                    ["enum"] = new[] { "query", "insert", "update", "delete" }
                },
                ["table"] = new Dictionary<string, object> { ["type"] = "string" },
                ["data"] = new Dictionary<string, object> { ["type"] = "object" }
            }
        };
    }

    public Dictionary<string, object> GetOutputSchema()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = new Dictionary<string, object>
            {
                ["success"] = new Dictionary<string, object> { ["type"] = "boolean" },
                ["result"] = new Dictionary<string, object> { ["type"] = "any" },
                ["rowsAffected"] = new Dictionary<string, object> { ["type"] = "number" }
            }
        };
    }
}