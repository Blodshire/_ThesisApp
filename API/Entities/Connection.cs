namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
        }

        public Connection(string connectionId, string loginName)
        {
            ConnectionId = connectionId;
            LoginName = loginName;
        }

        public string ConnectionId { get; set; }
        public string LoginName { get; set; }
    }
}
