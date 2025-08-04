using Microsoft.AspNetCore.Mvc;
using Pronetheia.Api.Services;

namespace Pronetheia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IAgentOrchestrationService _orchestrationService;

    public AgentController(IAgentOrchestrationService orchestrationService)
    {
        _orchestrationService = orchestrationService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> SendChatMessage([FromBody] ChatRequest request)
    {
        try
        {
            var response = await _orchestrationService.RouteUserMessage(request.Message, request.UserId ?? "anonymous");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetAgentStatuses()
    {
        try
        {
            var statuses = await _orchestrationService.GetAgentStatuses();
            return Ok(statuses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{agentId}/execute")]
    public async Task<IActionResult> ExecuteTask(string agentId, [FromBody] object task)
    {
        try
        {
            var response = await _orchestrationService.ExecuteAgentTask(agentId, task);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
}