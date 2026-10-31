using KhoLinhKien.Data;
using KhoLinhKien.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Controllers;

[Authorize(Roles = "QuanLy,ThuKho")]
public class PhieuNhapController : Controller
{
    private readonly ApplicationDbContext _context;

    public PhieuNhapController(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task NapDanhSachAsync(int? nccId = null)
    {
        ViewData["NhaCungCapId"] = new SelectList(
            await _context.NhaCungCap.OrderBy(x => x.TenNhaCungCap).ToListAsync(),
            "Id", "TenNhaCungCap", nccId);

        ViewData["LinhKiens"] = await _context.LinhKien
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

    // Danh sách phiếu nhập
    public async Task<IActionResult> Index()
    {
        var ds = await _context.PhieuNhap
            .Include(x => x.NhaCungCap)
            .Include(x => x.ChiTiets)
            .OrderByDescending(x => x.NgayNhap)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
        return View(ds);
    }

    // Xem chi tiết một phiếu
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var phieu = await _context.PhieuNhap
            .Include(x => x.NhaCungCap)
            .Include(x => x.ChiTiets).ThenInclude(c => c.LinhKien)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (phieu == null) return NotFound();
        return View(phieu);
    }

    public async Task<IActionResult> Create()
    {
        await NapDanhSachAsync();
        return View(new PhieuNhapCreateVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PhieuNhapCreateVM vm)
    {
        // Bỏ các dòng chưa chọn linh kiện
        vm.Dongs = vm.Dongs.Where(d => d.LinhKienId > 0).ToList();

        if (!vm.Dongs.Any())
            ModelState.AddModelError(string.Empty, "Phiếu nhập phải có ít nhất 1 dòng linh kiện.");

        if (vm.Dongs.GroupBy(d => d.LinhKienId).Any(g => g.Count() > 1))
            ModelState.AddModelError(string.Empty, "Mỗi linh kiện chỉ được xuất hiện 1 lần trong phiếu.");

        if (vm.Dongs.Any(d => d.SoLuong <= 0))
            ModelState.AddModelError(string.Empty, "Số lượng mỗi dòng phải lớn hơn 0.");

        if (vm.Dongs.Any(d => d.DonGia < 0))
            ModelState.AddModelError(string.Empty, "Đơn giá nhập không được âm.");

        // Kiểm tra nhà cung cấp và linh kiện có thật
        if (vm.NhaCungCapId != null && !await _context.NhaCungCap.AnyAsync(x => x.Id == vm.NhaCungCapId))
            ModelState.AddModelError(nameof(vm.NhaCungCapId), "Nhà cung cấp không tồn tại.");

        var ids = vm.Dongs.Select(d => d.LinhKienId).ToList();
        var linhKiens = await _context.LinhKien.Where(x => ids.Contains(x.Id)).ToListAsync();
        if (linhKiens.Count != ids.Count)
            ModelState.AddModelError(string.Empty, "Có linh kiện trong phiếu không tồn tại.");

        if (!ModelState.IsValid)
        {
            await NapDanhSachAsync(vm.NhaCungCapId);
            return View(vm);
        }

        // Lưu phiếu và cộng tồn trong CÙNG MỘT transaction: hoặc cùng thành công, hoặc cùng hủy
        using var tran = await _context.Database.BeginTransactionAsync();
        try
        {
            var phieu = new PhieuNhap
            {
                NhaCungCapId = vm.NhaCungCapId!.Value,
                NgayNhap = vm.NgayNhap,
                GhiChu = vm.GhiChu?.Trim(),
                NguoiLap = User.Identity?.Name
            };

            foreach (var d in vm.Dongs)
            {
                var lk = linhKiens.First(x => x.Id == d.LinhKienId);
                phieu.ChiTiets.Add(new ChiTietPhieuNhap
                {
                    LinhKienId = lk.Id,
                    SoLuong = d.SoLuong,
                    DonGia = d.DonGia
                });
                lk.SoLuongTon += d.SoLuong;   // Tăng tồn kho
            }

            _context.PhieuNhap.Add(phieu);
            await _context.SaveChangesAsync();
            await tran.CommitAsync();

            TempData["ThongBao"] = $"Đã lập phiếu nhập PN-{phieu.Id:D4} và cộng tồn kho.";
            return RedirectToAction(nameof(Details), new { id = phieu.Id });
        }
        catch
        {
            await tran.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Có lỗi khi lưu phiếu, chưa có thay đổi nào được ghi. Vui lòng thử lại.");
            await NapDanhSachAsync(vm.NhaCungCapId);
            return View(vm);
        }
    }
}