using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;   // Thêm dòng này
using Microsoft.AspNetCore.Mvc;
using KhoLinhKien.Models;

namespace KhoLinhKien.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = "QuanLy")]   // Chỉ vai trò QuanLy được vào trang này
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}