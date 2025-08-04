using Microsoft.Extensions.Logging;

namespace Pronetheia.Api.Services.MCP;

public class CodeGenerationMCP : IMCPTool
{
    private readonly IOpenRouterService _openRouterService;
    private readonly ILogger _logger;

    public string Id => "code-generation";
    public string Name => "CodeGenerationMCP";
    public string Category => "code_gen";
    public string Description => "AI-powered code generation, analysis, refactoring, test generation, and syntax validation";
    public string SecurityLevel => "safe";
    public int ExecutionTimeout => 60000;

    public CodeGenerationMCP(IOpenRouterService openRouterService, ILogger<CodeGenerationMCP> logger)
    {
        _openRouterService = openRouterService;
        _logger = logger;
    }

    public async Task<ToolExecutionResult> Execute(Dictionary<string, object> parameters)
    {
        try
        {
            var action = parameters.GetValueOrDefault("action")?.ToString() ?? "";
            var prompt = parameters.GetValueOrDefault("prompt")?.ToString();
            var language = parameters.GetValueOrDefault("language")?.ToString() ?? "csharp";
            var code = parameters.GetValueOrDefault("code")?.ToString();

            object? result = action.ToLower() switch
            {
                "generate" => await GenerateCode(prompt ?? "", language),
                "analyze" => await AnalyzeCode(code ?? "", language),
                "refactor" => await RefactorCode(code ?? "", prompt ?? ""),
                "test" => await GenerateTests(code ?? "", language),
                "validate" => await ValidateSyntax(code ?? "", language),
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
            _logger.LogError(ex, "Code generation operation failed");
            return new ToolExecutionResult
            {
                Success = false,
                ToolName = Name,
                Error = ex.Message,
                SecurityLevel = SecurityLevel
            };
        }
    }

    private async Task<object> GenerateCode(string prompt, string language)
    {
        var aiPrompt = $@"Generate {language} code for the following requirement:
{prompt}

Requirements:
- Use modern {language} best practices
- Include proper error handling
- Add necessary imports/using statements
- Make the code production-ready

Return ONLY the code without explanations.";

        var generatedCode = await _openRouterService.SendMessage(aiPrompt);
        
        return new
        {
            code = generatedCode,
            language = language,
            prompt = prompt,
            generated = true
        };
    }

    private async Task<object> AnalyzeCode(string code, string language)
    {
        var aiPrompt = $@"Analyze the following {language} code and provide insights:
```{language}
{code}
```

Provide analysis for:
1. Code quality and best practices
2. Potential bugs or issues
3. Performance considerations
4. Security concerns
5. Suggested improvements

Format as JSON.";

        var analysis = await _openRouterService.SendMessage(aiPrompt);
        
        return new
        {
            code = code,
            language = language,
            analysis = analysis,
            analyzed = true
        };
    }

    private async Task<object> RefactorCode(string code, string requirements)
    {
        var aiPrompt = $@"Refactor the following code according to these requirements:
{requirements}

Original code:
```
{code}
```

Return the refactored code with improvements.";

        var refactoredCode = await _openRouterService.SendMessage(aiPrompt);
        
        return new
        {
            original = code,
            refactored = refactoredCode,
            requirements = requirements
        };
    }

    private async Task<object> GenerateTests(string code, string language)
    {
        var aiPrompt = $@"Generate comprehensive unit tests for the following {language} code:
```{language}
{code}
```

Requirements:
- Cover all public methods
- Include edge cases
- Use appropriate testing framework for {language}
- Include both positive and negative test cases

Return ONLY the test code.";

        var testCode = await _openRouterService.SendMessage(aiPrompt);
        
        return new
        {
            sourceCode = code,
            testCode = testCode,
            language = language,
            framework = GetTestFramework(language)
        };
    }

    private async Task<object> ValidateSyntax(string code, string language)
    {
        // Simple validation - in production would use actual compiler/interpreter
        var isValid = !string.IsNullOrWhiteSpace(code);
        var issues = new List<string>();
        
        if (code.Count(c => c == '{') != code.Count(c => c == '}'))
        {
            issues.Add("Mismatched braces");
            isValid = false;
        }
        
        if (code.Count(c => c == '(') != code.Count(c => c == ')'))
        {
            issues.Add("Mismatched parentheses");
            isValid = false;
        }
        
        return await Task.FromResult(new
        {
            valid = isValid,
            language = language,
            issues = issues,
            codeLength = code.Length
        });
    }

    private string GetTestFramework(string language)
    {
        return language.ToLower() switch
        {
            "csharp" or "c#" => "xUnit",
            "javascript" or "typescript" => "Jest",
            "python" => "pytest",
            "java" => "JUnit",
            _ => "unknown"
        };
    }

    public async Task<bool> ValidateParameters(Dictionary<string, object> parameters)
    {
        var action = parameters.GetValueOrDefault("action")?.ToString();
        
        if (string.IsNullOrEmpty(action))
        {
            return false;
        }
        
        var validActions = new[] { "generate", "analyze", "refactor", "test", "validate" };
        var isValid = validActions.Contains(action.ToLower());
        
        // Check action-specific requirements
        if (isValid)
        {
            switch (action.ToLower())
            {
                case "generate":
                    isValid = parameters.ContainsKey("prompt");
                    break;
                case "analyze":
                case "test":
                case "validate":
                    isValid = parameters.ContainsKey("code");
                    break;
                case "refactor":
                    isValid = parameters.ContainsKey("code") && parameters.ContainsKey("prompt");
                    break;
            }
        }
        
        return await Task.FromResult(isValid);
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
                    ["enum"] = new[] { "generate", "analyze", "refactor", "test", "validate" }
                },
                ["prompt"] = new Dictionary<string, object> { ["type"] = "string" },
                ["language"] = new Dictionary<string, object> { ["type"] = "string" },
                ["code"] = new Dictionary<string, object> { ["type"] = "string" }
            },
            ["required"] = new[] { "action" }
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
                ["code"] = new Dictionary<string, object> { ["type"] = "string" },
                ["analysis"] = new Dictionary<string, object> { ["type"] = "object" },
                ["error"] = new Dictionary<string, object> { ["type"] = "string" }
            }
        };
    }
}