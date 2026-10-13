using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KhoLinhKien.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()   // Bật phân quyền theo vai trò
    .AddEntityFrameworkStores<ApplicationDbContext>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Tạo 3 vai trò nếu chưa có
    foreach (var role in new[] { "QuanLy", "ThuKho", "KyThuatVien" })
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));

    // Hàm tạo tài khoản mẫu rồi gán vai trò
    async Task TaoUser(string email, string role)
    {
        if (await userMgr.FindByEmailAsync(email) != null) return;   // Đã có thì bỏ qua
        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        await userMgr.CreateAsync(user, "Abc@12345");                // Mật khẩu mẫu
        await userMgr.AddToRoleAsync(user, role);
    }

    await TaoUser("quanly@kho.vn", "QuanLy");
    await TaoUser("thukho@kho.vn", "ThuKho");
    await TaoUser("kythuat@kho.vn", "KyThuatVien");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
