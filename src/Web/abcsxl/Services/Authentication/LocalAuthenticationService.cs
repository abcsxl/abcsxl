using System.Security.Claims;
using abcsxl.Data;
using abcsxl.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace abcsxl.Services.Authentication
{
    public class LocalAuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LocalAuthenticationService> _logger;

        public LocalAuthenticationService(
            ApplicationDbContext context,
            ILogger<LocalAuthenticationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AuthResult> SignInAsync(
            HttpContext httpContext,
            string usernameOrEmail,
            string password,
            bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                return AuthResult.Failure("用户名或密码错误");
            }

            var identifier = usernameOrEmail.Trim();
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == identifier ||
                    (u.Email != null && u.Email == identifier));

            if (user == null)
            {
                _logger.LogWarning("登录失败：找不到用户 {Identifier}", identifier);
                return AuthResult.Failure("用户名或密码错误");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("登录失败：用户 {Username} 已停用", user.Username);
                return AuthResult.Failure("账号已停用，请联系管理员");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("登录失败：用户 {Username} 密码错误", user.Username);
                return AuthResult.Failure("用户名或密码错误");
            }

            var role = user.Role.ToString();
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("用户 {Username} 登录成功", user.Username);

            return AuthResult.Success(new AuthenticatedUser(
                user.Id,
                user.Username,
                user.Email ?? string.Empty,
                role,
                user.DisplayName));
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<AuthenticatedUser?> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(idClaim, out var userId))
            {
                return null;
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new AuthenticatedUser(
                user.Id,
                user.Username,
                user.Email ?? string.Empty,
                user.Role.ToString(),
                user.DisplayName);
        }
    }
}
