using KhoLinhKien.Data;
using KhoLinhKien.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Controllers;

[Authorize(Roles = "QuanLy,ThuKho,KyThuatVien")]
public class LinhKienController : Controller
{
    private readonly ApplicationDbContext _context;

    public LinhKienController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Nạp 2 danh sách thả xuống cho form thêm/sửa
    private void NapDanhSach(int? loaiId = null, int? viTriId = null)
    {
        ViewData["LoaiLinhKienId"] = new SelectList(_context.LoaiLinhKien.OrderBy(x => x.TenLoai), "Id", "TenLoai", loaiId);
        ViewData["ViTriKhoId"] = new SelectList(_context.ViTriKho.OrderBy(x => x.TenViTri), "Id", "TenViTri", viTriId);
    }

    // Danh sách + tìm theo mã/tên + lọc theo loại, vị trí
    public async Task<IActionResult> Index(string? tuKhoa, int? loaiId, int? viTriId)
    {
        var query = _context.LinhKien
            .Include(x => x.LoaiLinhKien)
            .Include(x => x.ViTriKho)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            tuKhoa = tuKhoa.Trim();
            query = query.Where(x => x.MaLinhKien.Contains(tuKhoa) || x.TenLinhKien.Contains(tuKhoa));
        }
        if (loaiId != null) query = query.Where(x => x.LoaiLinhKienId == loaiId);
        if (viTriId != null) query = query.Where(x => x.ViTriKhoId == viTriId);

        ViewData["TuKhoa"] = tuKhoa;
        ViewData["LoaiId"] = new SelectList(_context.LoaiLinhKien.OrderBy(x => x.TenLoai), "Id", "TenLoai", loaiId);
        ViewData["ViTriId"] = new SelectList(_context.ViTriKho.OrderBy(x => x.TenViTri), "Id", "TenViTri", viTriId);

        return View(await query.OrderBy(x => x.MaLinhKien).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var lk = await _context.LinhKien
            .Include(x => x.LoaiLinhKien)
            .Include(x => x.ViTriKho)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (lk == null) return NotFound();
        return View(lk);
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public IActionResult Create()
    {
        NapDanhSach();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Create([Bind("MaLinhKien,TenLinhKien,LoaiLinhKienId,ViTriKhoId,DongMayTuongThich,DonGia,TonToiThieu")] LinhKien lk)
    {
        lk.MaLinhKien = (lk.MaLinhKien ?? string.Empty).Trim().ToUpper();

        if (await _context.LinhKien.AnyAsync(x => x.MaLinhKien == lk.MaLinhKien))
            ModelState.AddModelError(nameof(lk.MaLinhKien), "Mã linh kiện đã tồn tại.");

        if (!ModelState.IsValid)
        {
            NapDanhSach(lk.LoaiLinhKienId, lk.ViTriKhoId);
            return View(lk);
        }

        lk.SoLuongTon = 0;   // Tồn kho chỉ tăng khi lập phiếu nhập
        _context.Add(lk);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã thêm linh kiện.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var lk = await _context.LinhKien.FindAsync(id);
        if (lk == null) return NotFound();
        NapDanhSach(lk.LoaiLinhKienId, lk.ViTriKhoId);
        return View(lk);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,MaLinhKien,TenLinhKien,LoaiLinhKienId,ViTriKhoId,DongMayTuongThich,DonGia,TonToiThieu")] LinhKien model)
    {
        if (id != model.Id) return NotFound();
        model.MaLinhKien = (model.MaLinhKien ?? string.Empty).Trim().ToUpper();

        if (await _context.LinhKien.AnyAsync(x => x.MaLinhKien == model.MaLinhKien && x.Id != id))
            ModelState.AddModelError(nameof(model.MaLinhKien), "Mã linh kiện đã tồn tại.");

        if (!ModelState.IsValid)
        {
            NapDanhSach(model.LoaiLinhKienId, model.ViTriKhoId);
            return View(model);
        }

        var lk = await _context.LinhKien.FindAsync(id);
        if (lk == null) return NotFound();

        // Cập nhật từng trường, KHÔNG đụng tới SoLuongTon
        lk.MaLinhKien = model.MaLinhKien;
        lk.TenLinhKien = model.TenLinhKien;
        lk.LoaiLinhKienId = model.LoaiLinhKienId;
        lk.ViTriKhoId = model.ViTriKhoId;
        lk.DongMayTuongThich = model.DongMayTuongThich;
        lk.DonGia = model.DonGia;
        lk.TonToiThieu = model.TonToiThieu;

        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã cập nhật linh kiện.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var lk = await _context.LinhKien
            .Include(x => x.LoaiLinhKien)
            .Include(x => x.ViTriKho)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (lk == null) return NotFound();
        return View(lk);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "QuanLy,ThuKho")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lk = await _context.LinhKien.FindAsync(id);
        if (lk == null) return RedirectToAction(nameof(Index));

        bool daDung = await _context.ChiTietPhieuNhap.AnyAsync(x => x.LinhKienId == id)
                   || await _context.ChiTietDonSuaChua.AnyAsync(x => x.LinhKienId == id);

        if (daDung || lk.SoLuongTon > 0)
        {
            TempData["Loi"] = "Không thể xóa: linh kiện còn tồn kho hoặc đã có trong phiếu nhập / đơn sửa chữa.";
            return RedirectToAction(nameof(Index));
        }

        _context.LinhKien.Remove(lk);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Đã xóa linh kiện.";
        return RedirectToAction(nameof(Index));
    }
}