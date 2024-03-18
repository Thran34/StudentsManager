using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Context;
using StudentsManager.Domain.Models;

namespace StudentsManager.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _applicationDbContext;

    public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IActionResult> Index()
    {
        var user = await GetCurrentUserAsync();
        if (user != null)
        {
            ViewBag.UserFirstName = user.FirstName;
            ViewBag.UserLastName = user.LastName;
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user;
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}