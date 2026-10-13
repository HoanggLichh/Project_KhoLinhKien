namespace KhoLinhKien.Models;

public class ViTriKho
{
    public int Id { get; set; }
    public string TenViTri { get; set; } = string.Empty;   // Ví dụ: Kệ A - Ngăn 2
    public string? GhiChu { get; set; }
}
