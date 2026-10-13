namespace KhoLinhKien.Models;

public class DonSuaChua
{
    public int Id { get; set; }
    public string TenKhach { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string TenMay { get; set; } = string.Empty;       // VD: iPhone 11
    public string MoTaLoi { get; set; } = string.Empty;
    public DateTime NgayNhan { get; set; } = DateTime.Now;
    public string TrangThai { get; set; } = "NhanMay";   // NhanMay, DangSua, HoanTat

    public List<ChiTietDonSuaChua> ChiTiet { get; set; } = new();
}
