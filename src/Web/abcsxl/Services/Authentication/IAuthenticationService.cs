using System.Security.Claims;

namespace abcsxl.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthResult> SignInAsync(
            HttpContext httpContext,
            string usernameOrEmail,
            string password,
            bool rememberMe);

        Task SignOutAsync(HttpContext httpContext);

        Task<AuthenticatedUser?> GetCurrentUserAsync(ClaimsPrincipal principal);
    }
}
