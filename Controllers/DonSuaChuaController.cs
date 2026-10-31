using KhoLinhKien.Data;
using KhoLinhKien.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Controllers;

[Authorize(Roles = "QuanLy,ThuKho,KyThuatVien")]
public class DonSuaChuaController : Controller
{
    private readonly ApplicationDbContext _context;

    public DonSuaChuaController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? trangThai)
    {
        var q = _context.DonSuaChua.Include(x => x.ChiTiets).AsQueryable();

        if (!string.IsNullOrEmpty(trangThai))
            q = q.Where(x => x.TrangThai == trangThai);

        ViewData["TrangThai"] = trangThai;
        var ds = await q.OrderByDescending(x => x.NgayNhan).ThenByDescending(x => x.Id).ToListAsync();
        return View(ds);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var don = await _context.DonSuaChua
            .Include(x => x.ChiTiets).ThenInclude(c => c.LinhKien)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (don == null) return NotFound();
        return View(don);
    }

    [Authorize(Roles = "QuanLy,KyThuatVien")]
    public IActionResult Create()
    {
        return View(new DonSuaChua());
    }

    [Authorize(Roles = "QuanLy,KyThuatVien")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("TenKhach,SoDienThoai,TenMay,MoTaLoi,NgayNhan")] DonSuaChua don)
    {
        // Các trường hệ thống tự gán, không nhận từ form
        don.TrangThai = TrangThaiDon.NhanMay;
        don.CongTho = 0;
        don.NguoiLap = User.Identity?.Name;
        don.TenKhach = don.TenKhach?.Trim() ?? "";
        don.TenMay = don.TenMay?.Trim() ?? "";

        if (!ModelState.IsValid) return View(don);

        _context.DonSuaChua.Add(don);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Đã tạo đơn sửa chữa WO-{don.Id:D3}.";
        return RedirectToAction(nameof(Details), new { id = don.Id });
    }
}