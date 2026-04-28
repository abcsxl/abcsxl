using abcsxl.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace abcsxl.Controllers
{
    public class AccountController : Controller
    {
        // ===== 登录 =====
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // 已登录用户直接跳转
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: 验证用户名密码
            // 示例代码，实际应从数据库验证
            if (model.Email == "admin@example.com" && model.Password == "123456")
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, "用户名或密码错误");
            return View(model);
        }

        // ===== 退出登录 =====
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ===== 个人资料 =====
        [HttpGet]
        public IActionResult Profile()
        {
            ViewData["Title"] = "个人资料";
            ViewBag.UserRole = "Admin";
            ViewBag.UserEmail = User.Identity?.Name ?? "admin@example.com";

            var model = new ProfileViewModel
            {
                UserName = User.Identity?.Name ?? "admin",
                DisplayName = "管理员",
                Email = User.Identity?.Name ?? "admin@example.com",
                Phone = "13800138000",
                Bio = "热爱技术，专注.NET开发"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: 保存到数据库
            TempData["Success"] = "资料更新成功！";

            return RedirectToAction(nameof(Profile));
        }

        // ===== 修改密码 =====
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewData["Title"] = "修改密码";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: 验证旧密码，更新新密码
            ViewBag.Success = true;

            return View();
        }
    }
}
