namespace API.Helpers.PaginationHelperParams
{
    public class UserParams : PaginationParams
    {

        public string CurrentLogin { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;

        public string OrderBy { get; set; } = "lastActive";
    }
}
