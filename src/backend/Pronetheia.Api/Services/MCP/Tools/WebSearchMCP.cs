using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Pronetheia.Api.Services.MCP;

public class WebSearchMCP : IMCPTool
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;

    public string Id => "web-search";
    public string Name => "WebSearchMCP";
    public string Category => "web_search";
    public string Description => "Web search and content analysis including search, content fetching, website analysis, and documentation lookup";
    public string SecurityLevel => "safe";
    public int ExecutionTimeout => 20000;

    public WebSearchMCP(IHttpClientFactory httpClientFactory, ILogger<WebSearchMCP> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ToolExecutionResult> Execute(Dictionary<string, object> parameters)
    {
        try
        {
            var action = parameters.GetValueOrDefault("action")?.ToString() ?? "";
            
            object? result = action.ToLower() switch
            {
                "search" => await SearchWeb(parameters),
                "fetch" => await FetchContent(parameters),
                "documentation" => await FindDocumentation(parameters),
                _ => throw new ArgumentException($"Unknown action: {action}")
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
            _logger.LogError(ex, "Web search operation failed");
            return new ToolExecutionResult
            {
                Success = false,
                ToolName = Name,
                Error = ex.Message,
                SecurityLevel = SecurityLevel
            };
        }
    }

    private async Task<object> SearchWeb(Dictionary<string, object> parameters)
    {
        var query = parameters.GetValueOrDefault("query")?.ToString() ?? "";
        
        // Simulated search results
        var results = new[]
        {
            new { title = "Result 1", url = "https://example.com/1", snippet = $"Search result for: {query}" },
            new { title = "Result 2", url = "https://example.com/2", snippet = $"Another result for: {query}" }
        };
        
        return await Task.FromResult(new { query, results, count = results.Length });
    }

    private async Task<object> FetchContent(Dictionary<string, object> parameters)
    {
        var url = parameters.GetValueOrDefault("url")?.ToString() ?? "";
        
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL is required");
        
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        
        try
        {
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            return new
            {
                url,
                statusCode = (int)response.StatusCode,
                contentLength = content.Length,
                preview = content.Length > 500 ? content.Substring(0, 500) + "..." : content
            };
        }
        catch (HttpRequestException ex)
        {
            return new { url, error = ex.Message, fetched = false };
        }
    }

    private async Task<object> FindDocumentation(Dictionary<string, object> parameters)
    {
        var technology = parameters.GetValueOrDefault("technology")?.ToString() ?? "";
        
        // Simulated documentation links
        var docs = new[]
        {
            new { technology, title = $"{technology} Official Documentation", url = $"https://docs.{technology.ToLower()}.com" },
            new { technology, title = $"{technology} Tutorial", url = $"https://learn.{technology.ToLower()}.com" }
        };
        
        return await Task.FromResult(new { technology, documentation = docs });
    }

    public async Task<bool> ValidateParameters(Dictionary<string, object> parameters)
    {
        var action = parameters.GetValueOrDefault("action")?.ToString();
        return await Task.FromResult(!string.IsNullOrEmpty(action));
    }

    public Dictionary<string, object> GetInputSchema()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = new Dictionary<string, object>
            {
                ["action"] = new Dictionary<string, object>
                {
                    ["type"] = "string",
                    ["enum"] = new[] { "search", "fetch", "documentation" }
                },
                ["query"] = new Dictionary<string, object> { ["type"] = "string" },
                ["url"] = new Dictionary<string, object> { ["type"] = "string" }
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
                ["results"] = new Dictionary<string, object> { ["type"] = "array" },
                ["content"] = new Dictionary<string, object> { ["type"] = "string" }
            }
        };
    }
}