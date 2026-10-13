namespace KhoLinhKien.Models;

public class NhaCungCap
{
    public int Id { get; set; }
    public string TenNhaCungCap { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }   // Dấu ? nghĩa là được để trống
    public string? DiaChi { get; set; }
}
