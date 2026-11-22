L-Inventory – Quản lý kho linh kiện điện tử

Hệ thống web quản lý kho linh kiện cho cửa hàng sửa chữa điện thoại: nhập kho, xuất linh kiện theo đơn sửa chữa, tự trừ tồn, cảnh báo sắp hết, báo cáo và lịch sử nhập xuất.

Đồ án môn Thực hành chuyên ngành Công nghệ phần mềm.

Tính năng
Quản lý linh kiện, loại linh kiện, nhà cung cấp, vị trí kho
Phiếu nhập kho (tồn tăng đúng số lượng)
Đơn sửa chữa: xuất linh kiện, tự trừ tồn, xóa dòng xuất thì hoàn lại, chặn xuất quá tồn
Dashboard: thống kê, biểu đồ doanh thu, chuông cảnh báo linh kiện sắp hết
Báo cáo tồn kho (lọc sắp hết/hết hàng, tổng giá trị, xuất CSV)
Lịch sử nhập xuất từng linh kiện, đối chiếu với số tồn
Tìm kiếm nâng cao: lọc theo tình trạng, loại, vị trí; sắp xếp theo tên, tồn, giá
Phân quyền 3 vai trò: Quản lý, Thủ kho, Kỹ thuật viên
Tài khoản: ảnh đại diện (tối đa 2 MB), đổi mật khẩu
Giao diện nền đen, chuyển được sang chế độ sáng
Công nghệ

ASP.NET Core MVC (Razor) · Entity Framework Core (Code-First) · SQL Server LocalDB · ASP.NET Core Identity · Bootstrap 5

Yêu cầu
.NET SDK (phiên bản trùng với TargetFramework trong file .csproj)
SQL Server LocalDB (có sẵn khi cài Visual Studio, hoặc dùng SQL Server Express)
Công cụ EF: dotnet tool install --global dotnet-ef
Cách chạy
bash
git clone https://github.com/HoanggLichh/Project_KhoLinhKien.git
cd Project_KhoLinhKien
dotnet ef database update
dotnet run

Mở địa chỉ hiển thị trên terminal (ví dụ https://localhost:xxxx).

Nếu appsettings.json không có trong repo, tạo file với chuỗi kết nối:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=KhoLinhKien;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
Tài khoản mẫu
Vai trò	Tên đăng nhập	Mật khẩu
Quản lý	điền	điền
Thủ kho	điền	điền
Kỹ thuật viên	điền	điền
Cấu trúc thư mục
Controllers/   xử lý nghiệp vụ và phân quyền
Models/        các lớp thực thể
Data/          DbContext, migrations
Views/         giao diện Razor
wwwroot/       CSS, JS, ảnh (wwwroot/avatars/ không đưa lên Git)
Ảnh minh họa
<!-- Chèn ảnh: Dashboard, Danh sách linh kiện, Lịch sử linh kiện -->
Tiến độ
Giai đoạn	Nội dung	Trạng thái
1	Phân tích, ERD, database, đăng nhập	Xong
2	Linh kiện, loại, nhà cung cấp, vị trí kho	Xong
3	Phiếu nhập, đơn sửa chữa, Dashboard	Xong
4	Báo cáo, lịch sử, tìm kiếm, giao diện mới, kiểm thử	Xong
5	Kiểm thử toàn bộ, báo cáo, demo	Đang làm
Tác giả

Hoàng Lịch (HoanggLichh)
