using System.Diagnostics;
using KhoLinhKien.Data;
using KhoLinhKien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

        public async Task<IActionResult> Index()
    {
        var vm = new DashboardVM();
        if (User.Identity?.IsAuthenticated != true) return View(vm);

        static decimal TongTien(DonSuaChua d) => d.CongTho + d.ChiTiets.Sum(c => c.SoLuong * c.DonGia);

        vm.TongLinhKien = await _context.LinhKien.SumAsync(x => (int?)x.SoLuongTon) ?? 0;
        vm.SapHetHang = await _context.LinhKien.CountAsync(x => x.SoLuongTon <= x.TonToiThieu);
        vm.DonHoanTat = await _context.DonSuaChua.CountAsync(x => x.TrangThai == TrangThaiDon.HoanTat);
        vm.DonDangSua = await _context.DonSuaChua.CountAsync(x => x.TrangThai == TrangThaiDon.DangSua);

        // Lấy các đơn hoàn tất trong 14 ngày gần nhất (7 ngày này và 7 ngày trước đó)
        var homNay = DateTime.Today;
        var tuNgay = homNay.AddDays(-13);
        var donGanDay14 = await _context.DonSuaChua.AsNoTracking()
            .Include(x => x.ChiTiets)
            .Where(x => x.TrangThai == TrangThaiDon.HoanTat && x.NgayNhan >= tuNgay && x.NgayNhan < homNay.AddDays(1))
            .ToListAsync();

        // Biểu đồ 7 ngày gần nhất
        for (int i = 6; i >= 0; i--)
        {
            var ngay = homNay.AddDays(-i);
            vm.Ngay7.Add(new DoanhThuNgayVM
            {
                Nhan = ngay.DayOfWeek switch
                {
                    DayOfWeek.Monday => "T2",
                    DayOfWeek.Tuesday => "T3",
                    DayOfWeek.Wednesday => "T4",
                    DayOfWeek.Thursday => "T5",
                    DayOfWeek.Friday => "T6",
                    DayOfWeek.Saturday => "T7",
                    _ => "CN"
                },
                GiaTri = donGanDay14.Where(d => d.NgayNhan.Date == ngay).Sum(TongTien)
            });
        }

        vm.DoanhThuTuanNay = vm.Ngay7.Sum(x => x.GiaTri);
        vm.DoanhThuTuanTruoc = donGanDay14.Where(d => d.NgayNhan.Date < homNay.AddDays(-6)).Sum(TongTien);
        vm.PhanTramTuan = vm.DoanhThuTuanTruoc > 0
            ? (vm.DoanhThuTuanNay - vm.DoanhThuTuanTruoc) / vm.DoanhThuTuanTruoc * 100
            : null;

        // Doanh thu tháng này
        var dauThang = new DateTime(homNay.Year, homNay.Month, 1);
        var donThang = await _context.DonSuaChua.AsNoTracking()
            .Include(x => x.ChiTiets)
            .Where(x => x.TrangThai == TrangThaiDon.HoanTat && x.NgayNhan >= dauThang)
            .ToListAsync();
        vm.DoanhThuThang = donThang.Sum(TongTien);

        vm.DonGanDay = await _context.DonSuaChua.AsNoTracking()
            .Include(x => x.ChiTiets)
            .OrderByDescending(x => x.Id)
            .Take(3)
            .ToListAsync();

        vm.CanhBao = await _context.LinhKien.AsNoTracking()
            .Include(x => x.ViTriKho)
            .Where(x => x.SoLuongTon <= x.TonToiThieu)
            .OrderBy(x => x.SoLuongTon)
            .Take(5)
            .ToListAsync();

        return View(vm);
    }
    public IActionResult GioiThieu() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}