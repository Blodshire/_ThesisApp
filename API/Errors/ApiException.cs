namespace API.Errors
{
    public class ApiException
    {
        public int statusCode { get; set; }
        public string Message { get; set; }
        public string details { get; set; }

        public ApiException(int statusCode, string message, string details)
        {
            this.statusCode = statusCode;
            Message = message;
            this.details = details;
        }
    }
}
