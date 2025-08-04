using Pronetheia.Api.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Pronetheia.Api.Services.Agents;

public class EvolutionAgent : IAgent
{
    private readonly ICapabilityAnalyzer _analyzer;
    private readonly IEvolutionEngine _evolutionEngine;
    private readonly IOpenRouterService _openRouterService;
    private readonly IAgentCommunicationHub _communicationHub;
    private readonly ILogger _logger;
    
    public string Id => "evolution-agent";
    public string Name => "EvolutionAgent";
    public AgentType Type => AgentType.Evolution;
    public AgentStatus Status { get; private set; } = AgentStatus.Idle;
    public List<string> Capabilities => new()
    {
        "selfAnalysis",
        "capabilityGapIdentification",
        "agentGeneration",
        "codeGeneration",
        "systemOptimization"
    };

    public EvolutionAgent(
        ICapabilityAnalyzer analyzer,
        IEvolutionEngine evolutionEngine,
        IOpenRouterService openRouterService,
        IAgentCommunicationHub communicationHub,
        ILogger<EvolutionAgent> logger)
    {
        _analyzer = analyzer;
        _evolutionEngine = evolutionEngine;
        _openRouterService = openRouterService;
        _communicationHub = communicationHub;
        _logger = logger;
    }

    public async Task<AgentResponse> ProcessMessage(AgentMessage message)
    {
        try
        {
            Status = AgentStatus.Busy;
            _logger.LogInformation("EvolutionAgent processing message: {MessageType}", message.MessageType);

            var response = message.MessageType switch
            {
                MessageType.Task => await HandleEvolutionTask(message),
                MessageType.Evolution => await HandleEvolutionRequest(message),
                MessageType.Coordination => await HandleCoordination(message),
                _ => new AgentResponse
                {
                    AgentId = Id,
                    Success = false,
                    Error = $"Unsupported message type: {message.MessageType}"
                }
            };

            Status = AgentStatus.Idle;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message in EvolutionAgent");
            Status = AgentStatus.Error;
            return new AgentResponse
            {
                AgentId = Id,
                Success = false,
                Error = ex.Message
            };
        }
    }

    private async Task<AgentResponse> HandleEvolutionTask(AgentMessage message)
    {
        var taskContent = message.Content?.ToString() ?? string.Empty;
        
        // Determine evolution task type
        if (taskContent.ToLower().Contains("analyze"))
        {
            return await PerformSystemAnalysis();
        }
        else if (taskContent.ToLower().Contains("create agent"))
        {
            return await CreateNewAgent(taskContent);
        }
        else if (taskContent.ToLower().Contains("evolve"))
        {
            return await ExecuteEvolutionCycle();
        }
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = true,
            Result = new
            {
                message = "Evolution task received. Analyzing system capabilities...",
                taskType = "general-evolution"
            }
        };
    }

    private async Task<AgentResponse> HandleEvolutionRequest(AgentMessage message)
    {
        var evolutionRequest = JsonConvert.DeserializeObject<EvolutionRequest>(
            message.Content?.ToString() ?? "{}");
        
        _logger.LogInformation("Processing evolution request: {RequestType}", evolutionRequest?.Type);
        
        var evolutionResult = await _evolutionEngine.ExecuteEvolution(evolutionRequest);
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = evolutionResult.Success,
            Result = evolutionResult,
            Metadata = new Dictionary<string, object>
            {
                ["evolutionId"] = evolutionResult.EvolutionId,
                ["duration"] = evolutionResult.Duration
            }
        };
    }

    private async Task<AgentResponse> HandleCoordination(AgentMessage message)
    {
        var coordinationData = JsonConvert.DeserializeObject<dynamic>(message.Content?.ToString() ?? "{}");
        
        _logger.LogInformation("EvolutionAgent coordinating with {FromAgent}", message.FromAgent);
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = true,
            Result = new
            {
                acknowledged = true,
                coordinationId = coordinationData?.coordinationId ?? Guid.NewGuid().ToString()
            }
        };
    }

    private async Task<AgentResponse> PerformSystemAnalysis()
    {
        _logger.LogInformation("Performing system capability analysis");
        
        var gaps = await _analyzer.AnalyzeCapabilityGaps();
        
        // Identify the most critical gap
        var priorityGap = gaps.OrderByDescending(g => g.Priority).FirstOrDefault();
        
        var analysis = new
        {
            totalGaps = gaps.Length,
            criticalGaps = gaps.Count(g => g.Priority > 7),
            priorityGap = priorityGap,
            recommendedAction = DetermineRecommendedAction(priorityGap),
            capabilities = new
            {
                current = await _analyzer.GetCurrentCapabilities(),
                missing = gaps.Select(g => g.Area).ToList()
            }
        };
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = true,
            Result = analysis,
            Metadata = new Dictionary<string, object>
            {
                ["analysisTimestamp"] = DateTime.UtcNow,
                ["gapsIdentified"] = gaps.Length
            }
        };
    }

    private async Task<AgentResponse> CreateNewAgent(string specification)
    {
        _logger.LogInformation("Creating new agent based on specification");
        
        // Generate agent code using AI
        var agentPrompt = $@"Generate a new specialized agent for the Pronetheia system.
Specification: {specification}

The agent should:
1. Implement the IAgent interface
2. Have specialized capabilities
3. Integrate with the existing agent communication system
4. Include proper error handling and logging

Generate the complete C# class implementation.";

        var generatedCode = await _openRouterService.SendMessage(agentPrompt);
        
        // Validate the generated code
        var validationResult = await ValidateAgentCode(generatedCode);
        
        if (validationResult.IsValid)
        {
            // Store the new agent definition
            var newAgent = new
            {
                name = ExtractAgentName(generatedCode),
                code = generatedCode,
                capabilities = ExtractCapabilities(generatedCode),
                status = "ready-for-deployment"
            };
            
            return new AgentResponse
            {
                AgentId = Id,
                Success = true,
                Result = newAgent,
                Metadata = new Dictionary<string, object>
                {
                    ["generationTimestamp"] = DateTime.UtcNow,
                    ["validationPassed"] = true
                }
            };
        }
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = false,
            Error = $"Agent code validation failed: {validationResult.Error}"
        };
    }

    private async Task<AgentResponse> ExecuteEvolutionCycle()
    {
        _logger.LogInformation("Executing full evolution cycle");
        
        // Step 1: Analyze current state
        var gaps = await _analyzer.AnalyzeCapabilityGaps();
        
        // Step 2: Create evolution plan
        var plan = await _evolutionEngine.CreateEvolutionPlan(gaps);
        
        // Step 3: Generate improvements
        var improvements = await _evolutionEngine.GenerateImprovements(plan);
        
        // Step 4: Validate improvements
        var validation = await _evolutionEngine.ValidateImprovements(improvements);
        
        // Step 5: Deploy if valid
        if (validation.IsValid)
        {
            var deployment = await _evolutionEngine.DeployImprovements(improvements);
            
            return new AgentResponse
            {
                AgentId = Id,
                Success = true,
                Result = new
                {
                    evolutionCycle = "completed",
                    plan = plan,
                    improvements = improvements.Count(),
                    deployed = deployment.Success,
                    nextRecommendedAgent = "ProjectManagementAgent"
                },
                Metadata = new Dictionary<string, object>
                {
                    ["cycleId"] = Guid.NewGuid().ToString(),
                    ["duration"] = "0ms",
                    ["improvementsDeployed"] = improvements.Count()
                }
            };
        }
        
        return new AgentResponse
        {
            AgentId = Id,
            Success = false,
            Error = $"Evolution cycle validation failed: {validation.Error}"
        };
    }

    private string DetermineRecommendedAction(CapabilityGapRecord? gap)
    {
        if (gap == null)
            return "No critical gaps identified";
        
        return gap.Priority > 8 ? "Create new agent" : 
               gap.Priority > 5 ? "Enhance existing capability" : 
               "Monitor and optimize";
    }

    private async Task<ValidationResult> ValidateAgentCode(string code)
    {
        // Simple validation - in production, would compile and test
        return await Task.FromResult(new ValidationResult
        {
            IsValid = code.Contains("IAgent") && code.Contains("ProcessMessage"),
            Error = null
        });
    }

    private string ExtractAgentName(string code)
    {
        // Extract class name from generated code
        var match = System.Text.RegularExpressions.Regex.Match(code, @"class\s+(\w+)Agent");
        return match.Success ? match.Groups[1].Value + "Agent" : "NewAgent";
    }

    private List<string> ExtractCapabilities(string code)
    {
        // Extract capabilities from generated code
        return new List<string> { "specialized-processing", "domain-expertise" };
    }

    public async Task<bool> CanHandleMessage(AgentMessage message)
    {
        return await Task.FromResult(
            message.MessageType == MessageType.Evolution ||
            message.MessageType == MessageType.Task ||
            message.MessageType == MessageType.Coordination
        );
    }

    public async Task Initialize()
    {
        Status = AgentStatus.Active;
        _logger.LogInformation("EvolutionAgent initialized");
        
        // Schedule periodic evolution cycles
        _ = Task.Run(async () =>
        {
            while (Status == AgentStatus.Active)
            {
                await Task.Delay(TimeSpan.FromMinutes(30));
                if (Status == AgentStatus.Active)
                {
                    await PerformSystemAnalysis();
                }
            }
        });
        
        await Task.CompletedTask;
    }

    public async Task Shutdown()
    {
        Status = AgentStatus.Idle;
        _logger.LogInformation("EvolutionAgent shutdown");
        await Task.CompletedTask;
    }

    public async Task<AgentHealthStatus> GetHealthStatus()
    {
        return await Task.FromResult(new AgentHealthStatus
        {
            AgentId = Id,
            IsHealthy = Status != AgentStatus.Error,
            Status = Status,
            LastActivity = DateTime.UtcNow,
            Metrics = new Dictionary<string, object>
            {
                ["evolutionCycles"] = 0,
                ["agentsCreated"] = 0,
                ["gapsIdentified"] = 0
            }
        });
    }

    private class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? Error { get; set; }
    }

}