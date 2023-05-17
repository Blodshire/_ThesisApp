namespace API.Helpers.PaginationHelperParams
{
    public class MessageParams: PaginationParams
    {
        public string LoginName { get; set; }
        public string Container { get; set; } = "Unread";
    }
}
