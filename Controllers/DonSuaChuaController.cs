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

        if (don.TrangThai != TrangThaiDon.HoanTat)
        {
            ViewData["LinhKiens"] = await _context.LinhKien
                .AsNoTracking()
                .OrderBy(x => x.MaLinhKien)
                .Select(x => new LinhKienChonVM
                {
                    Id = x.Id,
                    Ten = x.MaLinhKien + " - " + x.TenLinhKien,
                    DonGia = x.DonGia,
                    Ton = x.SoLuongTon
                })
                .ToListAsync();
        }
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

    // ===== PHẦN B: xuất linh kiện cho đơn =====

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemLinhKien(int id, int linhKienId, int soLuong)
    {
        var don = await _context.DonSuaChua.FirstOrDefaultAsync(x => x.Id == id);
        if (don == null) return NotFound();

        if (don.TrangThai == TrangThaiDon.HoanTat)
        {
            TempData["Loi"] = "Đơn đã hoàn tất, không thể xuất thêm linh kiện.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (linhKienId <= 0)
        {
            TempData["Loi"] = "Vui lòng chọn linh kiện.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (soLuong <= 0)
        {
            TempData["Loi"] = "Số lượng xuất phải lớn hơn 0.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var lk = await _context.LinhKien.AsNoTracking().FirstOrDefaultAsync(x => x.Id == linhKienId);
        if (lk == null)
        {
            TempData["Loi"] = "Linh kiện không tồn tại.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (soLuong > lk.SoLuongTon)
        {
            TempData["Loi"] = $"Không đủ tồn kho: {lk.TenLinhKien} chỉ còn {lk.SoLuongTon}, không thể xuất {soLuong}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        using var tran = await _context.Database.BeginTransactionAsync();
        try
        {
            // Trừ tồn có điều kiện ngay trong database: chỉ trừ khi còn đủ hàng
            var daTru = await _context.LinhKien
                .Where(x => x.Id == linhKienId && x.SoLuongTon >= soLuong)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.SoLuongTon, x => x.SoLuongTon - soLuong));

            if (daTru == 0)
            {
                await tran.RollbackAsync();
                TempData["Loi"] = $"Không đủ tồn kho cho {lk.TenLinhKien}. Vui lòng kiểm tra lại số lượng.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var dong = await _context.ChiTietDonSuaChua
                .FirstOrDefaultAsync(x => x.DonSuaChuaId == id && x.LinhKienId == linhKienId);

            if (dong == null)
            {
                _context.ChiTietDonSuaChua.Add(new ChiTietDonSuaChua
                {
                    DonSuaChuaId = id,
                    LinhKienId = linhKienId,
                    SoLuong = soLuong,
                    DonGia = lk.DonGia
                });
            }
            else
            {
                dong.SoLuong += soLuong;   // Cùng linh kiện thì cộng dồn vào 1 dòng
            }

            await _context.SaveChangesAsync();
            await tran.CommitAsync();
        }
        catch
        {
            await tran.RollbackAsync();
            TempData["Loi"] = "Có lỗi khi xuất linh kiện, chưa có thay đổi nào được ghi. Vui lòng thử lại.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var conLai = lk.SoLuongTon - soLuong;
        TempData["ThongBao"] = $"Đã xuất {soLuong} x {lk.TenLinhKien} cho đơn WO-{id:D3}. Tồn còn {conLai}.";

        if (conLai <= lk.TonToiThieu)
            TempData["Loi"] = $"Cảnh báo: {lk.TenLinhKien} còn {conLai}, đã chạm hoặc dưới mức tối thiểu ({lk.TonToiThieu}). Cần nhập thêm.";

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaDong(int id, int chiTietId)
    {
        var don = await _context.DonSuaChua.FirstOrDefaultAsync(x => x.Id == id);
        if (don == null) return NotFound();

        if (don.TrangThai == TrangThaiDon.HoanTat)
        {
            TempData["Loi"] = "Đơn đã hoàn tất, không thể thay đổi linh kiện.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var dong = await _context.ChiTietDonSuaChua
            .FirstOrDefaultAsync(x => x.Id == chiTietId && x.DonSuaChuaId == id);
        if (dong == null)
        {
            TempData["Loi"] = "Không tìm thấy dòng linh kiện.";
            return RedirectToAction(nameof(Details), new { id });
        }

        using var tran = await _context.Database.BeginTransactionAsync();
        try
        {
            var linhKienId = dong.LinhKienId;
            var soLuong = dong.SoLuong;

            // Hoàn trả tồn kho
            await _context.LinhKien
                .Where(x => x.Id == linhKienId)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.SoLuongTon, x => x.SoLuongTon + soLuong));

            _context.ChiTietDonSuaChua.Remove(dong);
            await _context.SaveChangesAsync();
            await tran.CommitAsync();

            TempData["ThongBao"] = $"Đã bỏ dòng linh kiện khỏi đơn và hoàn lại {soLuong} vào kho.";
        }
        catch
        {
            await tran.RollbackAsync();
            TempData["Loi"] = "Có lỗi khi bỏ dòng linh kiện, chưa có thay đổi nào được ghi.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

        // ===== PHẦN C: trạng thái và tiền công =====

    [Authorize(Roles = "QuanLy,KyThuatVien")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(int id, string trangThai)
    {
        var don = await _context.DonSuaChua.FirstOrDefaultAsync(x => x.Id == id);
        if (don == null) return NotFound();

        // Chỉ cho đi tới: Nhận máy -> Đang sửa -> Hoàn tất
        var hopLe =
            (don.TrangThai == TrangThaiDon.NhanMay && trangThai == TrangThaiDon.DangSua) ||
            (don.TrangThai == TrangThaiDon.DangSua && trangThai == TrangThaiDon.HoanTat);

        if (!hopLe)
        {
            TempData["Loi"] = "Không thể chuyển đơn sang trạng thái này.";
            return RedirectToAction(nameof(Details), new { id });
        }

        don.TrangThai = trangThai;
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Đơn WO-{id:D3} đã chuyển sang: {TrangThaiDon.Ten(trangThai)}.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "QuanLy,KyThuatVien")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatCong(int id, decimal congTho)
    {
        var don = await _context.DonSuaChua.FirstOrDefaultAsync(x => x.Id == id);
        if (don == null) return NotFound();

        if (don.TrangThai == TrangThaiDon.HoanTat)
        {
            TempData["Loi"] = "Đơn đã hoàn tất, không thể sửa tiền công.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (congTho < 0 || congTho > 1000000000)
        {
            TempData["Loi"] = "Tiền công không hợp lệ.";
            return RedirectToAction(nameof(Details), new { id });
        }

        don.CongTho = congTho;
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Đã lưu tiền công {congTho:N0}đ cho đơn WO-{id:D3}.";
        return RedirectToAction(nameof(Details), new { id });
    }

}