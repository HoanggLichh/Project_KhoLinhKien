using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KhoLinhKien.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập tên tài khoản hoặc email")]
            [Display(Name = "Tên tài khoản hoặc email")]
            public string TenDangNhap { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            public string MatKhau { get; set; } = string.Empty;

            public bool GhiNho { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid) return Page();

            // Cho phép đăng nhập bằng email hoặc tên tài khoản
            var user = await _userManager.FindByEmailAsync(Input.TenDangNhap)
                       ?? await _userManager.FindByNameAsync(Input.TenDangNhap);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    user, Input.MatKhau, Input.GhiNho, lockoutOnFailure: false);

                if (result.Succeeded) return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Tên tài khoản hoặc mật khẩu không đúng.");
            return Page();
        }
    }
}