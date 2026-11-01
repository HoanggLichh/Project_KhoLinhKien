namespace KhoLinhKien.Models;

public class DoanhThuNgayVM
{
    public string Nhan { get; set; } = string.Empty;   // T2, T3 ... CN
    public decimal GiaTri { get; set; }
}

public class DashboardVM
{
    public int TongLinhKien { get; set; }
    public int SapHetHang { get; set; }
    public int DonHoanTat { get; set; }
    public int DonDangSua { get; set; }
    public decimal DoanhThuThang { get; set; }

    public List<DoanhThuNgayVM> Ngay7 { get; set; } = new();
    public decimal DoanhThuTuanNay { get; set; }
    public decimal DoanhThuTuanTruoc { get; set; }
    public decimal? PhanTramTuan { get; set; }   // null nếu tuần trước chưa có doanh thu

    public List<DonSuaChua> DonGanDay { get; set; } = new();
    public List<LinhKien> CanhBao { get; set; } = new();
}