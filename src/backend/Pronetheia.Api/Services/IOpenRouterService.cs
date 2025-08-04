using System.Text;
using Newtonsoft.Json;

namespace Pronetheia.Api.Services;

public class OpenRouterChatMessage
{
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;
}

public interface IOpenRouterService
{
    Task<string> SendMessage(string message);
    Task<string> SendMessage(List<OpenRouterChatMessage> messages);
}

public class OpenRouterService : IOpenRouterService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _model;

    public OpenRouterService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration["OpenRouter:ApiKey"] ?? throw new InvalidOperationException("OpenRouter API key not configured");
        _baseUrl = _configuration["OpenRouter:BaseUrl"] ?? "https://openrouter.ai/api/v1/chat/completions";
        _model = _configuration["OpenRouter:Model"] ?? "deepseek/deepseek-r1-0528:free";
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:3000");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "Pronetheia Agent System");
    }

    public async Task<string> SendMessage(string message)
    {
        var messages = new List<object>
        {
            new { role = "user", content = message }
        };
        
        return await SendChatRequest(messages);
    }

    public async Task<string> SendMessage(List<OpenRouterChatMessage> messages)
    {
        // Ensure we have a system message for English responses
        var systemMessage = new
        {
            role = "system",
            content = "You are Pronetheia, an AI agent system. Always respond in English. Be helpful, concise, and professional."
        };
        
        var openRouterMessages = new List<object> { systemMessage };
        
        openRouterMessages.AddRange(messages.Select(m => new
        {
            role = m.Role.ToLower(),
            content = m.Content
        }));
        
        return await SendChatRequest(openRouterMessages);
    }

    private async Task<string> SendChatRequest(List<object> messages)
    {
        try
        {
            var requestBody = new
            {
                model = _model,
                messages = messages,
                max_tokens = 2000,
                temperature = 0.7
            };
            
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(_baseUrl, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenRouter API error: {response.StatusCode} - {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<dynamic>(responseContent);
            
            var aiMessage = responseObj?.choices?[0]?.message?.content?.ToString();
            return aiMessage ?? "No response from AI";
        }
        catch (Exception ex)
        {
            return $"Error communicating with AI: {ex.Message}";
        }
    }

}