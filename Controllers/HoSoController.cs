using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KhoLinhKien.Controllers
{
    public class DoiMatKhauVM
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        public string MatKhauCu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [DataType(DataType.Password)]
        public string MatKhauMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string XacNhan { get; set; } = string.Empty;
    }

    public class HoSoVM
    {
        public string Ten { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public bool CoMatKhau { get; set; }
        public string? AvatarUrl { get; set; }
        public DoiMatKhauVM DoiMatKhau { get; set; } = new();
    }

    // Trang tài khoản + ảnh đại diện (lưu file theo Id tài khoản trong wwwroot/avatars)
    [Authorize]
    public class HoSoController : Controller
    {
        private static readonly string[] AvatarExts = { ".jpg", ".png", ".webp" };
        private const long GioiHanByte = 2 * 1024 * 1024;

        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HoSoController(IWebHostEnvironment env, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _env = env;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ---------- Trang tài khoản ----------
        [HttpGet]
        public async Task<IActionResult> Index() => View(await DungVM());

        private async Task<HoSoVM> DungVM()
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = new HoSoVM
            {
                Ten = user?.UserName ?? User.Identity?.Name ?? "",
                Email = user?.Email ?? "",
                VaiTro = User.IsInRole("QuanLy") ? "Quản lý"
                       : User.IsInRole("ThuKho") ? "Thủ kho"
                       : User.IsInRole("KyThuatVien") ? "Kỹ thuật viên" : "",
                CoMatKhau = user != null && await _userManager.HasPasswordAsync(user),
                AvatarUrl = LayAvatarUrl()
            };
            return vm;
        }

        // ---------- Đổi mật khẩu ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau([Bind(Prefix = "DoiMatKhau")] DoiMatKhauVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var kq = await _userManager.ChangePasswordAsync(user, model.MatKhauCu, model.MatKhauMoi);
                if (kq.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["ThongBao"] = "Đã đổi mật khẩu.";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var e in kq.Errors) ModelState.AddModelError(string.Empty, e.Description);
            }

            var vm = await DungVM();
            return View("Index", vm);
        }

        // ---------- Ảnh đại diện ----------
        private string ThuMucAnh()
        {
            var dir = Path.Combine(_env.WebRootPath, "avatars");
            Directory.CreateDirectory(dir);
            return dir;
        }

        private static void XoaAnhCu(string dir, string uid)
        {
            foreach (var e in AvatarExts)
            {
                var p = Path.Combine(dir, uid + e);
                if (System.IO.File.Exists(p)) System.IO.File.Delete(p);
            }
        }

        private string VeTrang(string? returnUrl) =>
            !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";

        private string? LayUid()
        {
            var uid = Path.GetFileName(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            return uid == "" ? null : uid;
        }

        private string? LayAvatarUrl()
        {
            var uid = LayUid();
            if (uid == null) return null;
            foreach (var e in AvatarExts)
            {
                var p = Path.Combine(_env.WebRootPath, "avatars", uid + e);
                if (System.IO.File.Exists(p))
                    return "/avatars/" + uid + e + "?v=" + System.IO.File.GetLastWriteTimeUtc(p).Ticks;
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiAnhDaiDien(IFormFile? anh, string? returnUrl)
        {
            var back = VeTrang(returnUrl);
            var uid = LayUid();
            if (uid == null) return Redirect(back);

            if (anh == null || anh.Length == 0) { TempData["Loi"] = "Bạn chưa chọn ảnh."; return Redirect(back); }
            if (anh.Length > GioiHanByte) { TempData["Loi"] = "Ảnh tối đa 2 MB."; return Redirect(back); }

            // Nhận dạng định dạng bằng nội dung file, không tin tên file người dùng gửi
            var head = new byte[12];
            await using (var s = anh.OpenReadStream())
            {
                var doc = await s.ReadAsync(head, 0, head.Length);
                if (doc < 12) { TempData["Loi"] = "File ảnh không hợp lệ."; return Redirect(back); }
            }

            string? ext = null;
            if (head[0] == 0xFF && head[1] == 0xD8 && head[2] == 0xFF) ext = ".jpg";
            else if (head[0] == 0x89 && head[1] == 0x50 && head[2] == 0x4E && head[3] == 0x47) ext = ".png";
            else if (head[0] == 'R' && head[1] == 'I' && head[2] == 'F' && head[3] == 'F'
                  && head[8] == 'W' && head[9] == 'E' && head[10] == 'B' && head[11] == 'P') ext = ".webp";

            if (ext == null) { TempData["Loi"] = "Chỉ nhận ảnh JPG, PNG hoặc WEBP."; return Redirect(back); }

            var dir = ThuMucAnh();
            XoaAnhCu(dir, uid);
            await using (var fs = new FileStream(Path.Combine(dir, uid + ext), FileMode.Create))
                await anh.CopyToAsync(fs);

            TempData["ThongBao"] = "Đã cập nhật ảnh đại diện.";
            return Redirect(back);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaAnhDaiDien(string? returnUrl)
        {
            var uid = LayUid();
            if (uid != null) XoaAnhCu(ThuMucAnh(), uid);
            TempData["ThongBao"] = "Đã gỡ ảnh đại diện.";
            return Redirect(VeTrang(returnUrl));
        }
    }
}