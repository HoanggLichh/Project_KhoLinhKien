using KhoLinhKien.Data;
using KhoLinhKien.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Controllers;

[Authorize(Roles = "QuanLy,ThuKho,KyThuatVien")]
public class NhaCungCapController : Controller
{
    private readonly ApplicationDbContext _context;

    public NhaCungCapController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.NhaCungCap.OrderBy(x => x.TenNhaCungCap).ToListAsync());
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Create([Bind("TenNhaCungCap,SoDienThoai,DiaChi")] NhaCungCap ncc)
    {
        if (!ModelState.IsValid) return View(ncc);

        _context.Add(ncc);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã thêm nhà cung cấp.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var ncc = await _context.NhaCungCap.FindAsync(id);
        if (ncc == null) return NotFound();
        return View(ncc);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TenNhaCungCap,SoDienThoai,DiaChi")] NhaCungCap ncc)
    {
        if (id != ncc.Id) return NotFound();
        if (!ModelState.IsValid) return View(ncc);

        _context.Update(ncc);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã cập nhật nhà cung cấp.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var ncc = await _context.NhaCungCap.FindAsync(id);
        if (ncc == null) return NotFound();
        return View(ncc);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ncc = await _context.NhaCungCap.FindAsync(id);
        if (ncc == null) return RedirectToAction(nameof(Index));

        if (await _context.PhieuNhap.AnyAsync(x => x.NhaCungCapId == id))
        {
            TempData["Loi"] = "Không thể xóa: nhà cung cấp này đã có phiếu nhập.";
            return RedirectToAction(nameof(Index));
        }

        _context.NhaCungCap.Remove(ncc);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã xóa nhà cung cấp.";
        return RedirectToAction(nameof(Index));
    }
}