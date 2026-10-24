using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KhoLinhKien.Data;

var builder = WebApplication.CreateBuilder(args);

// Kết nối SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity + phân quyền theo vai trò
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Đường dẫn khi chưa đăng nhập hoặc không đủ quyền
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Home/Privacy";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Tạo 3 vai trò và 3 tài khoản mẫu
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    foreach (var role in new[] { "QuanLy", "ThuKho", "KyThuatVien" })
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));

    async Task TaoUser(string email, string role)
    {
        if (await userMgr.FindByEmailAsync(email) != null) return;
        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        await userMgr.CreateAsync(user, "Abc@12345");
        await userMgr.AddToRoleAsync(user, role);
    }

    await TaoUser("quanly@kho.vn", "QuanLy");
    await TaoUser("thukho@kho.vn", "ThuKho");
    await TaoUser("kythuat@kho.vn", "KyThuatVien");
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();