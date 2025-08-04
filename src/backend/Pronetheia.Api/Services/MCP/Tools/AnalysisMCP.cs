using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Pronetheia.Api.Services.MCP;

public class AnalysisMCP : IMCPTool
{
    private readonly ILogger _logger;

    public string Id => "analysis";
    public string Name => "AnalysisMCP";
    public string Category => "analysis";
    public string Description => "Codebase analysis including pattern identification, capability assessment, dependency analysis, and metrics calculation";
    public string SecurityLevel => "safe";
    public int ExecutionTimeout => 30000;

    public AnalysisMCP(ILogger<AnalysisMCP> logger)
    {
        _logger = logger;
    }

    public async Task<ToolExecutionResult> Execute(Dictionary<string, object> parameters)
    {
        try
        {
            var analysisType = parameters.GetValueOrDefault("analysisType")?.ToString() ?? "";
            var targetPath = parameters.GetValueOrDefault("targetPath")?.ToString() ?? Directory.GetCurrentDirectory();
            var options = parameters.GetValueOrDefault("options") as Dictionary<string, object> ?? new();

            object? result = analysisType.ToLower() switch
            {
                "codebase" => await AnalyzeCodebase(targetPath),
                "patterns" => await IdentifyPatterns(targetPath),
                "capabilities" => await AssessCapabilities(targetPath),
                "dependencies" => await AnalyzeDependencies(targetPath),
                "metrics" => await CalculateMetrics(targetPath),
                _ => throw new ArgumentException($"Unknown analysis type: {analysisType}")
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
            _logger.LogError(ex, "Analysis operation failed");
            return new ToolExecutionResult
            {
                Success = false,
                ToolName = Name,
                Error = ex.Message,
                SecurityLevel = SecurityLevel
            };
        }
    }

    private async Task<object> AnalyzeCodebase(string path)
    {
        var analysis = new
        {
            path = path,
            files = CountFiles(path),
            structure = AnalyzeStructure(path),
            languages = DetectLanguages(path),
            totalLines = CountLines(path),
            timestamp = DateTime.UtcNow
        };
        
        return await Task.FromResult(analysis);
    }

    private async Task<object> IdentifyPatterns(string path)
    {
        var patterns = new List<object>();
        
        // Identify common design patterns
        if (Directory.Exists(Path.Combine(path, "Services")))
        {
            patterns.Add(new { pattern = "Service Layer", confidence = 0.9 });
        }
        
        if (Directory.Exists(Path.Combine(path, "Models")))
        {
            patterns.Add(new { pattern = "MVC/MVVM", confidence = 0.85 });
        }
        
        if (Directory.Exists(Path.Combine(path, "Repository")))
        {
            patterns.Add(new { pattern = "Repository Pattern", confidence = 0.8 });
        }
        
        return await Task.FromResult(new
        {
            path = path,
            patterns = patterns,
            totalPatterns = patterns.Count
        });
    }

    private async Task<object> AssessCapabilities(string path)
    {
        var capabilities = new List<object>();
        
        // Check for various capabilities
        if (File.Exists(Path.Combine(path, "Program.cs")))
        {
            capabilities.Add(new { capability = "ASP.NET Core Application", present = true });
        }
        
        if (Directory.Exists(Path.Combine(path, "Controllers")))
        {
            capabilities.Add(new { capability = "REST API", present = true });
        }
        
        if (Directory.Exists(Path.Combine(path, "Hubs")))
        {
            capabilities.Add(new { capability = "Real-time Communication (SignalR)", present = true });
        }
        
        if (Directory.Exists(Path.Combine(path, "Services", "Agents")))
        {
            capabilities.Add(new { capability = "Multi-Agent System", present = true });
        }
        
        return await Task.FromResult(new
        {
            path = path,
            capabilities = capabilities,
            totalCapabilities = capabilities.Count
        });
    }

    private async Task<object> AnalyzeDependencies(string path)
    {
        var dependencies = new List<object>();
        
        // Check for project files
        var csprojFiles = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
        foreach (var csproj in csprojFiles)
        {
            var content = await File.ReadAllTextAsync(csproj);
            var packageRefs = Regex.Matches(content, @"<PackageReference Include=""([^""]+)""");
            
            foreach (Match match in packageRefs)
            {
                dependencies.Add(new
                {
                    name = match.Groups[1].Value,
                    type = "NuGet",
                    project = Path.GetFileName(csproj)
                });
            }
        }
        
        return new
        {
            path = path,
            dependencies = dependencies,
            totalDependencies = dependencies.Count
        };
    }

    private async Task<object> CalculateMetrics(string path)
    {
        var metrics = new
        {
            path = path,
            loc = CountLines(path),
            files = CountFiles(path),
            directories = CountDirectories(path),
            complexity = EstimateComplexity(path),
            timestamp = DateTime.UtcNow
        };
        
        return await Task.FromResult(metrics);
    }

    private Dictionary<string, int> CountFiles(string path)
    {
        var counts = new Dictionary<string, int>();
        
        if (!Directory.Exists(path))
            return counts;
        
        counts["total"] = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
        counts["cs"] = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Length;
        counts["json"] = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories).Length;
        counts["tsx"] = Directory.GetFiles(path, "*.tsx", SearchOption.AllDirectories).Length;
        counts["ts"] = Directory.GetFiles(path, "*.ts", SearchOption.AllDirectories).Length;
        
        return counts;
    }

    private Dictionary<string, object> AnalyzeStructure(string path)
    {
        var structure = new Dictionary<string, object>();
        
        var dirs = Directory.GetDirectories(path);
        structure["topLevelDirectories"] = dirs.Select(d => Path.GetFileName(d)).ToList();
        structure["depth"] = CalculateMaxDepth(path);
        
        return structure;
    }

    private List<string> DetectLanguages(string path)
    {
        var languages = new HashSet<string>();
        
        if (Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Any())
            languages.Add("C#");
        if (Directory.GetFiles(path, "*.ts", SearchOption.AllDirectories).Any())
            languages.Add("TypeScript");
        if (Directory.GetFiles(path, "*.tsx", SearchOption.AllDirectories).Any())
            languages.Add("React TypeScript");
        if (Directory.GetFiles(path, "*.js", SearchOption.AllDirectories).Any())
            languages.Add("JavaScript");
        if (Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories).Any())
            languages.Add("SQL");
        
        return languages.ToList();
    }

    private int CountLines(string path)
    {
        if (!Directory.Exists(path))
            return 0;
        
        var totalLines = 0;
        var codeFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".cs") || f.EndsWith(".ts") || f.EndsWith(".tsx") || 
                       f.EndsWith(".js") || f.EndsWith(".jsx"));
        
        foreach (var file in codeFiles)
        {
            try
            {
                totalLines += File.ReadAllLines(file).Length;
            }
            catch { }
        }
        
        return totalLines;
    }

    private int CountDirectories(string path)
    {
        if (!Directory.Exists(path))
            return 0;
        
        return Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Length;
    }

    private int CalculateMaxDepth(string path, int currentDepth = 0)
    {
        if (!Directory.Exists(path) || currentDepth > 10)
            return currentDepth;
        
        var dirs = Directory.GetDirectories(path);
        if (dirs.Length == 0)
            return currentDepth;
        
        return dirs.Max(d => CalculateMaxDepth(d, currentDepth + 1));
    }

    private string EstimateComplexity(string path)
    {
        var fileCount = CountFiles(path)["total"];
        var lineCount = CountLines(path);
        
        if (fileCount < 10 && lineCount < 1000)
            return "Simple";
        if (fileCount < 50 && lineCount < 5000)
            return "Moderate";
        if (fileCount < 200 && lineCount < 20000)
            return "Complex";
        
        return "Very Complex";
    }

    public async Task<bool> ValidateParameters(Dictionary<string, object> parameters)
    {
        var analysisType = parameters.GetValueOrDefault("analysisType")?.ToString();
        
        if (string.IsNullOrEmpty(analysisType))
        {
            return false;
        }
        
        var validTypes = new[] { "codebase", "patterns", "capabilities", "dependencies", "metrics" };
        return await Task.FromResult(validTypes.Contains(analysisType.ToLower()));
    }

    public Dictionary<string, object> GetInputSchema()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = new Dictionary<string, object>
            {
                ["analysisType"] = new Dictionary<string, object>
                {
                    ["type"] = "string",
                    ["enum"] = new[] { "codebase", "patterns", "capabilities", "dependencies", "metrics" }
                },
                ["targetPath"] = new Dictionary<string, object> { ["type"] = "string" },
                ["options"] = new Dictionary<string, object> { ["type"] = "object" }
            },
            ["required"] = new[] { "analysisType" }
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
                ["analysis"] = new Dictionary<string, object> { ["type"] = "object" },
                ["error"] = new Dictionary<string, object> { ["type"] = "string" }
            }
        };
    }
}