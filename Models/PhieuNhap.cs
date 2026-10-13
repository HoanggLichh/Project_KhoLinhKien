namespace KhoLinhKien.Models;

public class PhieuNhap
{
    public int Id { get; set; }
    public DateTime NgayNhap { get; set; } = DateTime.Now;

    public int NhaCungCapId { get; set; }
    public NhaCungCap? NhaCungCap { get; set; }

    public List<ChiTietPhieuNhap> ChiTiet { get; set; } = new();   // Nhiều dòng chi tiết
}
    