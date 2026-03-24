// Hubs/NotificationHub.cs

using Microsoft.AspNetCore.SignalR;

namespace CaseInvestigationManagementSystem.Hubs
{
    public class NotificationHub : Hub
    {
        // Client connect hone pe
        public override async Task OnConnectedAsync()
        {
            // Role ke hisaab se group mein add karo
            var role = Context.GetHttpContext().Session.GetString("role");
            
            if(role == "Admin")
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, "Admin");
            
            if(role == "Investigator")
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, "Investigator");

            await base.OnConnectedAsync();
        }

        // Client disconnect hone pe
        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}