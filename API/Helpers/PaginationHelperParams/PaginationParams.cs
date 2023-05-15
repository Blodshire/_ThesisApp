namespace API.Helpers.PaginationHelperParams
{
    public class PaginationParams
    {
        private const int MaxPageSize = 40;
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
