using System.ComponentModel.DataAnnotations;

namespace KhoLinhKien.Models;

public class LoaiLinhKien
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên loại")]
    [StringLength(100, ErrorMessage = "Tên loại tối đa 100 ký tự")]
    [Display(Name = "Tên loại")]
    public string TenLoai { get; set; } = string.Empty;
}