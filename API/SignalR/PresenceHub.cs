using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            this.tracker = tracker;
        }

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            var isOnline = await tracker.UserConnectedAsync(Context.User.GetLoginName(), Context.ConnectionId);
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetLoginName());

            var currentUsers = await tracker.getOnlineUsersAsync();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var isOffline = await tracker.UserDisconnectedAsync(Context.User.GetLoginName(), Context.ConnectionId);
            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetLoginName());

            await base.OnDisconnectedAsync(ex);
        }
    }
}
