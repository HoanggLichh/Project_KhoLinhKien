namespace KhoLinhKien.Models;

public class ChiTietPhieuNhap
{
    public int Id { get; set; }
    public int PhieuNhapId { get; set; }

    public int LinhKienId { get; set; }
    public LinhKien? LinhKien { get; set; }

    public int SoLuong { get; set; }
    public decimal DonGiaNhap { get; set; }
}
