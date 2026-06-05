using abcsxl.Models.ViewModels.Account;
using abcsxl.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace abcsxl.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _auth;

        public AccountController(IAuthenticationService auth)
        {
            _auth = auth;
        }

        // ===== 登录 =====
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _auth.SignInAsync(HttpContext, model.Email, model.Password, model.RememberMe);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "登录失败");
                return View(model);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // ===== 退出登录 =====
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _auth.SignOutAsync(HttpContext);
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
