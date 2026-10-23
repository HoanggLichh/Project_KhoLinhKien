using System.ComponentModel.DataAnnotations;

namespace KhoLinhKien.Models;

public class ViTriKho
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên vị trí")]
    [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
    [Display(Name = "Tên vị trí")]
    public string TenViTri { get; set; } = string.Empty;   // Ví dụ: Kệ A - Ngăn 2

    [StringLength(250, ErrorMessage = "Tối đa 250 ký tự")]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
}