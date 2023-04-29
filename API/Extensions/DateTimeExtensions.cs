namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly DoB)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DoB.Year;

            if (DoB > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
