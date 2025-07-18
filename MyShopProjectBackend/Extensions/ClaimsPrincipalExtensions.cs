namespace MyShopProjectBackend.Extensions
{
    using MyShopProjectBackend.Exceptions;
    using System.Security.Claims;
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new AuthorizationException("Користувача не знайдено");
        }
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? throw new AuthorizationException("Ім'я користувача не знайдено");
        }


    }
}
