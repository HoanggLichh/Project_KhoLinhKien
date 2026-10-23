using KhoLinhKien.Data;
using KhoLinhKien.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Controllers;

[Authorize(Roles = "QuanLy,ThuKho,KyThuatVien")]
public class LoaiLinhKienController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoaiLinhKienController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.LoaiLinhKien.OrderBy(x => x.TenLoai).ToListAsync());
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Create([Bind("TenLoai")] LoaiLinhKien loai)
    {
        loai.TenLoai = (loai.TenLoai ?? string.Empty).Trim();
        if (await _context.LoaiLinhKien.AnyAsync(x => x.TenLoai == loai.TenLoai))
            ModelState.AddModelError(nameof(loai.TenLoai), "Loại này đã tồn tại.");
        if (!ModelState.IsValid) return View(loai);

        _context.Add(loai);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã thêm loại linh kiện.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var loai = await _context.LoaiLinhKien.FindAsync(id);
        if (loai == null) return NotFound();
        return View(loai);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TenLoai")] LoaiLinhKien loai)
    {
        if (id != loai.Id) return NotFound();
        loai.TenLoai = (loai.TenLoai ?? string.Empty).Trim();
        if (await _context.LoaiLinhKien.AnyAsync(x => x.TenLoai == loai.TenLoai && x.Id != id))
            ModelState.AddModelError(nameof(loai.TenLoai), "Loại này đã tồn tại.");
        if (!ModelState.IsValid) return View(loai);

        _context.Update(loai);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã cập nhật loại linh kiện.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var loai = await _context.LoaiLinhKien.FindAsync(id);
        if (loai == null) return NotFound();
        return View(loai);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var loai = await _context.LoaiLinhKien.FindAsync(id);
        if (loai == null) return RedirectToAction(nameof(Index));

        if (await _context.LinhKien.AnyAsync(x => x.LoaiLinhKienId == id))
        {
            TempData["Loi"] = "Không thể xóa: vẫn còn linh kiện thuộc loại này.";
            return RedirectToAction(nameof(Index));
        }

        _context.LoaiLinhKien.Remove(loai);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã xóa loại linh kiện.";
        return RedirectToAction(nameof(Index));
    }
}