<div align="center">

# L-Inventory

**Hệ thống web quản lý kho linh kiện điện tử cho cửa hàng sửa chữa điện thoại**

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-Code--First-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white)

Đồ án môn **Thực hành chuyên ngành Công nghệ phần mềm** · Trường Đại học Quy Nhơn

</div>

---

## Mục lục

- [Giới thiệu](#giới-thiệu)
- [Tính năng](#tính-năng)
- [Phân quyền](#phân-quyền)
- [Công nghệ](#công-nghệ)
- [Cài đặt và chạy](#cài-đặt-và-chạy)
- [Tài khoản demo](#tài-khoản-demo)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)
- [Ảnh minh họa](#ảnh-minh-họa)
- [Tiến độ](#tiến-độ)
- [Tác giả](#tác-giả)

## Giới thiệu

Cửa hàng sửa chữa điện thoại thường quản lý hàng trăm loại linh kiện nhỏ bằng sổ tay hoặc bảng tính, dẫn đến sai lệch tồn kho và khó truy vết khi có chênh lệch. **L-Inventory** gắn việc xuất linh kiện với đơn sửa chữa, tự động cập nhật tồn kho và cảnh báo khi linh kiện sắp hết.

## Tính năng

| Nhóm | Chức năng |
|---|---|
| Danh mục | Quản lý linh kiện, loại linh kiện, nhà cung cấp, vị trí kho |
| Nhập kho | Phiếu nhập kho, tồn tăng đúng số lượng nhập |
| Xuất kho | Đơn sửa chữa xuất linh kiện, tự trừ tồn, xóa dòng xuất thì hoàn lại, chặn xuất vượt tồn |
| Dashboard | Thống kê tổng quan, biểu đồ doanh thu, chuông cảnh báo linh kiện sắp hết |
| Báo cáo | Báo cáo tồn kho (lọc sắp hết, hết hàng), tổng giá trị tồn, xuất CSV |
| Truy vết | Lịch sử nhập xuất từng linh kiện, đối chiếu với số tồn |
| Tìm kiếm | Lọc theo tình trạng, loại, vị trí; sắp xếp theo tên, tồn kho, giá |
| Tài khoản | Ảnh đại diện (tối đa 2 MB), đổi mật khẩu |
| Giao diện | Nền đen mặc định, chuyển được sang chế độ sáng |

## Phân quyền

Hệ thống có 3 vai trò: **Quản lý**, **Thủ kho** và **Kỹ thuật viên**.

| Chức năng | Quản lý | Thủ kho | Kỹ thuật viên |
|---|:---:|:---:|:---:|
| Xem danh mục linh kiện | ✓ | ✓ | ✓ |
| Thêm, sửa, xóa danh mục | ✓ | ✓ | – |
| Lập phiếu nhập kho | ✓ | ✓ | – |
| Tạo đơn sửa chữa, xuất linh kiện | ✓ | – | ✓ |
| Báo cáo tồn kho, lịch sử | ✓ | ✓ | – |
| Dashboard, đổi mật khẩu, ảnh đại diện | ✓ | ✓ | ✓ |

## Công nghệ

- **Backend:** ASP.NET Core MVC (Razor), Entity Framework Core (Code-First)
- **Cơ sở dữ liệu:** SQL Server LocalDB
- **Xác thực:** ASP.NET Core Identity
- **Giao diện:** Bootstrap 5

## Cài đặt và chạy

### Yêu cầu

- .NET SDK (phiên bản trùng với `TargetFramework` trong file `.csproj`)
- SQL Server LocalDB (có sẵn khi cài Visual Studio) hoặc SQL Server Express
- Công cụ EF: `dotnet tool install --global dotnet-ef`

### Các bước

```bash
# 1. Lấy mã nguồn
git clone https://github.com/HoanggLichh/Project_KhoLinhKien.git
cd Project_KhoLinhKien

# 2. Tạo cơ sở dữ liệu
dotnet ef database update

# 3. Chạy ứng dụng
dotnet run
```

Mở địa chỉ hiển thị trên terminal (ví dụ `https://localhost:xxxx`).

### Cấu hình kết nối

Nếu repo không có `appsettings.json`, tạo file này ở thư mục dự án:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=KhoLinhKien;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## Tài khoản demo

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Quản lý | `<điền>` | `<điền>` |
| Thủ kho | `<điền>` | `<điền>` |
| Kỹ thuật viên | `<điền>` | `<điền>` |

> Chỉ dùng tài khoản demo cho buổi trình bày, không đưa mật khẩu thật lên GitHub.

## Cấu trúc thư mục

```text
Project_KhoLinhKien/
├── Controllers/   Xử lý nghiệp vụ và phân quyền
├── Models/        Các lớp thực thể
├── Data/          DbContext, migrations
├── Views/         Giao diện Razor
└── wwwroot/       CSS, JS, ảnh (wwwroot/avatars/ không đưa lên Git)
```

## Ảnh minh họa

| Dashboard | Danh sách linh kiện | Lịch sử linh kiện |
|---|---|---|
| ![Dashboard](docs/images/dashboard.png) | ![Linh kiện](docs/images/linh-kien.png) | ![Lịch sử](docs/images/lich-su.png) |

> Lưu ảnh chụp màn hình vào `docs/images/` với đúng tên trên để ảnh hiển thị.

## Tiến độ

| Giai đoạn | Nội dung | Trạng thái |
|:---:|---|:---:|
| 1 | Phân tích, ERD, cơ sở dữ liệu, đăng nhập | ✅ Xong |
| 2 | Linh kiện, loại, nhà cung cấp, vị trí kho | ✅ Xong |
| 3 | Phiếu nhập, đơn sửa chữa, Dashboard | ✅ Xong |
| 4 | Báo cáo, lịch sử, tìm kiếm, giao diện mới | ✅ Xong |
| 5 | Kiểm thử toàn bộ, báo cáo, demo | 🚧 Đang làm |

## Tác giả

**Hoàng Lịch** ([@HoanggLichh](https://github.com/HoanggLichh)) · Trường Đại học Quy Nhơn
