using KhoLinhKien.Data;
using KhoLinhKien.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Controllers;

[Authorize(Roles = "QuanLy,ThuKho,KyThuatVien")]
public class ViTriKhoController : Controller
{
    private readonly ApplicationDbContext _context;

    public ViTriKhoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.ViTriKho.OrderBy(x => x.TenViTri).ToListAsync());
    }

    // Xem các linh kiện đang nằm ở vị trí này (tra cứu)
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var viTri = await _context.ViTriKho.FindAsync(id);
        if (viTri == null) return NotFound();

        ViewBag.DanhSach = await _context.LinhKien
            .Include(x => x.LoaiLinhKien)
            .Where(x => x.ViTriKhoId == id)
            .OrderBy(x => x.MaLinhKien)
            .ToListAsync();

        var chuaGan = await _context.LinhKien
            .Where(x => x.ViTriKhoId == null)
            .OrderBy(x => x.MaLinhKien)
            .Select(x => new { x.Id, Ten = x.MaLinhKien + " - " + x.TenLinhKien })
            .ToListAsync();
        ViewBag.ChuaGan = new SelectList(chuaGan, "Id", "Ten");

        return View(viTri);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> GanLinhKien(int viTriId, int linhKienId)
    {
        var lk = await _context.LinhKien.FindAsync(linhKienId);
        if (lk != null)
        {
            lk.ViTriKhoId = viTriId;
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Đã gán linh kiện vào vị trí.";
        }
        return RedirectToAction(nameof(Details), new { id = viTriId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> BoLinhKien(int viTriId, int linhKienId)
    {
        var lk = await _context.LinhKien.FindAsync(linhKienId);
        if (lk != null && lk.ViTriKhoId == viTriId)
        {
            lk.ViTriKhoId = null;
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Đã bỏ linh kiện khỏi vị trí.";
        }
        return RedirectToAction(nameof(Details), new { id = viTriId });
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Create([Bind("TenViTri,GhiChu")] ViTriKho viTri)
    {
        viTri.TenViTri = (viTri.TenViTri ?? string.Empty).Trim();
        if (await _context.ViTriKho.AnyAsync(x => x.TenViTri == viTri.TenViTri))
            ModelState.AddModelError(nameof(viTri.TenViTri), "Vị trí này đã tồn tại.");
        if (!ModelState.IsValid) return View(viTri);

        _context.Add(viTri);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã thêm vị trí.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var viTri = await _context.ViTriKho.FindAsync(id);
        if (viTri == null) return NotFound();
        return View(viTri);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TenViTri,GhiChu")] ViTriKho viTri)
    {
        if (id != viTri.Id) return NotFound();
        viTri.TenViTri = (viTri.TenViTri ?? string.Empty).Trim();
        if (await _context.ViTriKho.AnyAsync(x => x.TenViTri == viTri.TenViTri && x.Id != id))
            ModelState.AddModelError(nameof(viTri.TenViTri), "Vị trí này đã tồn tại.");
        if (!ModelState.IsValid) return View(viTri);

        _context.Update(viTri);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã cập nhật vị trí.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var viTri = await _context.ViTriKho.FindAsync(id);
        if (viTri == null) return NotFound();
        return View(viTri);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var viTri = await _context.ViTriKho.FindAsync(id);
        if (viTri == null) return RedirectToAction(nameof(Index));

        if (await _context.LinhKien.AnyAsync(x => x.ViTriKhoId == id))
        {
            TempData["Loi"] = "Không thể xóa: vị trí này vẫn còn linh kiện.";
            return RedirectToAction(nameof(Index));
        }

        _context.ViTriKho.Remove(viTri);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã xóa vị trí.";
        return RedirectToAction(nameof(Index));
    }
}