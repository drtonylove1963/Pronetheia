using Microsoft.Extensions.Logging;

namespace Pronetheia.Api.Services.MCP;

public class FileOperationsMCP : IMCPTool
{
    private readonly ILogger _logger;
    private readonly string _basePath;

    public string Id => "file-operations";
    public string Name => "FileOperationsMCP";
    public string Category => "file_ops";
    public string Description => "File system operations including create, read, update, delete, and directory listing";
    public string SecurityLevel => "elevated";
    public int ExecutionTimeout => 10000;

    public FileOperationsMCP(ILogger<FileOperationsMCP> logger)
    {
        _logger = logger;
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
        
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<ToolExecutionResult> Execute(Dictionary<string, object> parameters)
    {
        try
        {
            var operation = parameters.GetValueOrDefault("operation")?.ToString() ?? "";
            var path = parameters.GetValueOrDefault("path")?.ToString() ?? "";
            var content = parameters.GetValueOrDefault("content")?.ToString();

            // Ensure path is within workspace
            var fullPath = GetSafePath(path);
            
            object? result = operation.ToLower() switch
            {
                "create" => await CreateFile(fullPath, content ?? ""),
                "read" => await ReadFile(fullPath),
                "update" => await UpdateFile(fullPath, content ?? ""),
                "delete" => await DeleteFile(fullPath),
                "list" => await ListDirectory(fullPath),
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
            _logger.LogError(ex, "File operation failed");
            return new ToolExecutionResult
            {
                Success = false,
                ToolName = Name,
                Error = ex.Message,
                SecurityLevel = SecurityLevel
            };
        }
    }

    private string GetSafePath(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_basePath, relativePath));
        
        // Ensure the path is within the workspace
        if (!fullPath.StartsWith(_basePath))
        {
            throw new UnauthorizedAccessException("Access to path outside workspace is forbidden");
        }
        
        return fullPath;
    }

    private async Task<object> CreateFile(string path, string content)
    {
        await File.WriteAllTextAsync(path, content);
        _logger.LogInformation("Created file: {Path}", path);
        return new { created = true, path = path, size = content.Length };
    }

    private async Task<object> ReadFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }
        
        var content = await File.ReadAllTextAsync(path);
        var info = new FileInfo(path);
        
        return new
        {
            path = path,
            content = content,
            size = info.Length,
            modified = info.LastWriteTimeUtc
        };
    }

    private async Task<object> UpdateFile(string path, string content)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }
        
        await File.WriteAllTextAsync(path, content);
        _logger.LogInformation("Updated file: {Path}", path);
        return new { updated = true, path = path, size = content.Length };
    }

    private async Task<object> DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            _logger.LogInformation("Deleted file: {Path}", path);
            return new { deleted = true, path = path };
        }
        else if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            _logger.LogInformation("Deleted directory: {Path}", path);
            return new { deleted = true, path = path, type = "directory" };
        }
        else
        {
            throw new FileNotFoundException($"Path not found: {path}");
        }
    }

    private async Task<object> ListDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            path = Path.GetDirectoryName(path) ?? _basePath;
        }
        
        var entries = new List<object>();
        
        foreach (var dir in Directory.GetDirectories(path))
        {
            var info = new DirectoryInfo(dir);
            entries.Add(new
            {
                name = info.Name,
                type = "directory",
                path = dir,
                modified = info.LastWriteTimeUtc
            });
        }
        
        foreach (var file in Directory.GetFiles(path))
        {
            var info = new FileInfo(file);
            entries.Add(new
            {
                name = info.Name,
                type = "file",
                path = file,
                size = info.Length,
                modified = info.LastWriteTimeUtc
            });
        }
        
        return await Task.FromResult(new
        {
            path = path,
            entries = entries,
            count = entries.Count
        });
    }

    public async Task<bool> ValidateParameters(Dictionary<string, object> parameters)
    {
        var operation = parameters.GetValueOrDefault("operation")?.ToString();
        var path = parameters.GetValueOrDefault("path")?.ToString();
        
        if (string.IsNullOrEmpty(operation) || string.IsNullOrEmpty(path))
        {
            return false;
        }
        
        var validOperations = new[] { "create", "read", "update", "delete", "list" };
        return await Task.FromResult(validOperations.Contains(operation.ToLower()));
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
                    ["enum"] = new[] { "create", "read", "update", "delete", "list" }
                },
                ["path"] = new Dictionary<string, object> { ["type"] = "string" },
                ["content"] = new Dictionary<string, object> { ["type"] = "string" }
            },
            ["required"] = new[] { "operation", "path" }
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
                ["error"] = new Dictionary<string, object> { ["type"] = "string" }
            }
        };
    }
}