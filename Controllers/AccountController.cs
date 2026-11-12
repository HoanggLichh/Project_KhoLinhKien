using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KhoLinhKien.Controllers
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản hoặc email")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = string.Empty;

        public bool GhiNho { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string XacNhanMatKhau { get; set; } = string.Empty;
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string XacNhanMatKhau { get; set; } = string.Empty;
    }

    [AllowAnonymous]
    public class AccountController : Controller
    {
        private const string VaiTroMacDinh = "KyThuatVien";
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly IWebHostEnvironment _env;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
            IAuthenticationSchemeProvider schemes, IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _schemes = schemes;
            _env = env;
        }

        private IActionResult VeTrangChu(string? returnUrl) =>
            !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl) : RedirectToAction("Index", "Home");

        // ---------- Đăng nhập ----------
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.TenDangNhap)
                       ?? await _userManager.FindByNameAsync(model.TenDangNhap);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.MatKhau, model.GhiNho, false);
                if (result.Succeeded) return VeTrangChu(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Tên tài khoản hoặc mật khẩu không đúng.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ---------- Tạo tài khoản ----------
        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email này đã được sử dụng.");
                return View(model);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.MatKhau);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, VaiTroMacDinh);
            await _signInManager.SignInAsync(user, false);
            TempData["ThongBao"] = "Tạo tài khoản thành công.";
            return RedirectToAction("Index", "Home");
        }

        // ---------- Quên mật khẩu ----------
        [HttpGet]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action(nameof(ResetPassword), "Account",
                    new { email = model.Email, token }, Request.Scheme);

                // TODO: gửi link qua email khi đã có SMTP. Hiện chỉ hiển thị khi đang phát triển.
                if (_env.IsDevelopment()) ViewData["DevLink"] = link;
            }

            
            // Luôn báo thành công để không lộ email nào đã tồn tại
            ViewData["DaGui"] = true;
            return View(model);
        }

        // ---------- Đặt lại mật khẩu ----------
        [HttpGet]
        public IActionResult ResetPassword(string? email, string? token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(Login));
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Liên kết không hợp lệ hoặc đã hết hạn.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.MatKhau);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
                return View(model);
            }

            TempData["ThongBao"] = "Đặt lại mật khẩu thành công. Hãy đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        // ---------- Đăng nhập Google ----------
        [HttpGet]
        public async Task<IActionResult> GoogleLogin(string? returnUrl = null)
        {
            if (await _schemes.GetSchemeAsync("Google") == null)
            {
                TempData["Loi"] = "Đăng nhập Google chưa được cấu hình.";
                return RedirectToAction(nameof(Login));
            }
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Account", new { returnUrl });
            var props = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            props.SetParameter("prompt", "select_account");
            return Challenge(props, "Google");
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction(nameof(Login));

            var r = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (r.Succeeded) return VeTrangChu(returnUrl);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                TempData["Loi"] = "Tài khoản Google không cung cấp email.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var created = await _userManager.CreateAsync(user);
                if (!created.Succeeded)
                {
                    TempData["Loi"] = "Không tạo được tài khoản từ Google.";
                    return RedirectToAction(nameof(Login));
                }
                await _userManager.AddToRoleAsync(user, VaiTroMacDinh);
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, false);
            return VeTrangChu(returnUrl);
        }
    }
}