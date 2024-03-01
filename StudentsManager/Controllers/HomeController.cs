using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Models;

namespace StudentsManager.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly Context.Context _context;

    public HomeController(UserManager<ApplicationUser> userManager, Context.Context context)
    {
        _userManager = userManager;
        _context = context;
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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user;
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}