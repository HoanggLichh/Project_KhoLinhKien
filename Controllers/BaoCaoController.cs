using System.Text;
using KhoLinhKien.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Controllers
{
    [Authorize(Roles = "QuanLy,ThuKho")]
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BaoCaoController(ApplicationDbContext context) => _context = context;

        private IQueryable<Models.LinhKien> Loc(string? tt)
        {
            var q = _context.LinhKien.Include(x => x.LoaiLinhKien).AsQueryable();
            if (tt == "saphet") q = q.Where(x => x.SoLuongTon > 0 && x.SoLuongTon <= x.TonToiThieu);
            else if (tt == "hethang") q = q.Where(x => x.SoLuongTon == 0);
            return q.OrderBy(x => x.MaLinhKien);
        }

        public async Task<IActionResult> TonKho(string? tt)
        {
            ViewBag.TT = tt;
            return View(await Loc(tt).ToListAsync());
        }

        public async Task<IActionResult> XuatCsv(string? tt)
        {
            var ds = await Loc(tt).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("Mã,Tên,Loại,Tồn,Tồn tối thiểu,Trạng thái");
            foreach (var x in ds)
            {
                var t = x.SoLuongTon == 0 ? "Hết hàng"
                      : x.SoLuongTon <= x.TonToiThieu ? "Sắp hết" : "Đủ hàng";
                sb.AppendLine($"{x.MaLinhKien},\"{x.TenLinhKien}\",\"{x.LoaiLinhKien?.TenLoai}\",{x.SoLuongTon},{x.TonToiThieu},{t}");
            }
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return File(bytes, "text/csv", $"ton-kho-{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}