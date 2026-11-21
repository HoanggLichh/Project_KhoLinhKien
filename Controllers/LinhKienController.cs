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

        // Danh sách + tìm theo mã/tên + lọc theo loại, vị trí, tình trạng tồn + sắp xếp
    public async Task<IActionResult> Index(string? tuKhoa, int? loaiId, int? viTriId, string? tinhTrang, string? sapXep)
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

        if (tinhTrang == "saphet")
            query = query.Where(x => x.SoLuongTon > 0 && x.SoLuongTon <= x.TonToiThieu);
        else if (tinhTrang == "hethang")
            query = query.Where(x => x.SoLuongTon <= 0);
        else if (tinhTrang == "duhang")
            query = query.Where(x => x.SoLuongTon > x.TonToiThieu);

        query = sapXep switch
        {
            "ton_tang" => query.OrderBy(x => x.SoLuongTon),
            "ton_giam" => query.OrderByDescending(x => x.SoLuongTon),
            "gia_tang" => query.OrderBy(x => x.DonGia),
            "gia_giam" => query.OrderByDescending(x => x.DonGia),
            "ten"      => query.OrderBy(x => x.TenLinhKien),
            _          => query.OrderBy(x => x.MaLinhKien)
        };

        ViewData["TuKhoa"] = tuKhoa;
        ViewData["TinhTrang"] = tinhTrang;
        ViewData["SapXep"] = sapXep;
        ViewData["LoaiId"] = new SelectList(_context.LoaiLinhKien.OrderBy(x => x.TenLoai), "Id", "TenLoai", loaiId);
        ViewData["ViTriId"] = new SelectList(_context.ViTriKho.OrderBy(x => x.TenViTri), "Id", "TenViTri", viTriId);

        return View(await query.ToListAsync());
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

        public async Task<IActionResult> LichSu(int id)
    {
        var lk = await _context.LinhKien.FindAsync(id);
        if (lk == null) return NotFound();

        var nhap = await _context.ChiTietPhieuNhap
            .Where(x => x.LinhKienId == id)
            .Select(x => new { Ngay = x.PhieuNhap!.NgayNhap, x.PhieuNhapId, x.SoLuong })
            .ToListAsync();

        var xuat = await _context.ChiTietDonSuaChua
            .Where(x => x.LinhKienId == id)
            .Select(x => new { Ngay = x.DonSuaChua!.NgayNhan, x.DonSuaChuaId, x.SoLuong })
            .ToListAsync();

        var ds = nhap.Select(x => new LichSuDong
                { Ngay = x.Ngay, Loai = "Nhập", ChungTu = $"PN-{x.PhieuNhapId:D4}", SoLuong = x.SoLuong })
            .Concat(xuat.Select(x => new LichSuDong
                { Ngay = x.Ngay, Loai = "Xuất", ChungTu = $"DSC-{x.DonSuaChuaId:D4}", SoLuong = x.SoLuong }))
            .OrderByDescending(x => x.Ngay)
            .ToList();

        ViewBag.LinhKien = lk;
        return View(ds);
    }
}