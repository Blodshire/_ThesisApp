using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetLoginName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            var Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int converted;
            if (int.TryParse(Id, out converted))
                return converted;
            return -1;

        }
    }
}
