using Microsoft.AspNetCore.SignalR;

namespace Pronetheia.Api.Hubs;

public class AgentHub : Hub
{
    public async Task JoinAgentGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveAgentGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendMessageToAgent(string agentId, object message)
    {
        await Clients.Group($"agent-{agentId}").SendAsync("ReceiveMessage", message);
    }

    public async Task BroadcastAgentStatus(object status)
    {
        await Clients.All.SendAsync("AgentStatusUpdate", status);
    }
}