namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();//loginName, List of connectionids O

        public Task<bool> UserConnectedAsync(string loginName, string connectionId)
        {
            bool isOnline = false;
            //Dictionary is not threadsafe, we need to lock it
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(loginName))
                    OnlineUsers[loginName].Add(connectionId);
                else
                {
                    OnlineUsers.Add(loginName, new List<string> { connectionId });
                    isOnline = true;
                }
                   
            }
            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnectedAsync(string loginName, string connectionId)
        {
            bool isOffline = false;
            //Dictionary is not threadsafe, we need to lock it
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(loginName))
                    return Task.FromResult(isOffline);

                OnlineUsers[loginName].Remove(connectionId);

                if (OnlineUsers[loginName].Count == 0)
                {
                    OnlineUsers.Remove(loginName);
                    isOffline= true;
                }
            }
            return Task.FromResult(isOffline);
        }

        public Task<string[]> getOnlineUsersAsync() {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                onlineUsers= OnlineUsers.OrderBy(k=> k.Key).Select(k=> k.Key).ToArray();
            }
            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUserAsync(string loginName) {
            List<string> connectionIds;

            lock(OnlineUsers)
            {
                connectionIds= OnlineUsers.GetValueOrDefault(loginName);
            }
            return Task.FromResult(connectionIds);
        }
    }
}
