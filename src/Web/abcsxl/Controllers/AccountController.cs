using abcsxl.Models.ViewModels.Account;
using abcsxl.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace abcsxl.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _auth;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(IAuthenticationService auth, IStringLocalizer<AccountController> localizer)
        {
            _auth = auth;
            _localizer = localizer;
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
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? _localizer["LoginFailed"].Value);
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
            ViewData["Title"] = _localizer["ProfileTitle"].Value;
            ViewBag.UserRole = "Admin";
            ViewBag.UserEmail = User.Identity?.Name ?? "admin@example.com";

            var model = new ProfileViewModel
            {
                UserName = User.Identity?.Name ?? "admin",
                DisplayName = _localizer["DefaultDisplayName"].Value,
                Email = User.Identity?.Name ?? "admin@example.com",
                Phone = "13800138000",
                Bio = _localizer["DefaultBio"].Value
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

            TempData["Success"] = _localizer["ProfileUpdated"].Value;

            return RedirectToAction(nameof(Profile));
        }

        // ===== 修改密码 =====
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewData["Title"] = _localizer["ChangePasswordTitle"].Value;
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

            ViewBag.Success = true;

            return View();
        }
    }
}
