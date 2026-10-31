namespace KhoLinhKien.Models;

public class ChiTietDonSuaChua
{
    public int Id { get; set; }

    public int DonSuaChuaId { get; set; }
    public DonSuaChua? DonSuaChua { get; set; }

    public int LinhKienId { get; set; }
    public LinhKien? LinhKien { get; set; }

    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }   // Giá tính cho khách
}