using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentsManager.Abstract;
using StudentsManager.Abstract.Service;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var result = await _userService.RegisterUserAsync(model);
        if (result.Succeeded) return RedirectToAction("index", "home");

        foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        var result = await _userService.LoginUserAsync(model);
        if (result.Succeeded) return LocalRedirect(returnUrl ?? Url.Content("~/"));

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTeacher(int teacherId)
    {
        var result = await _userService.DeleteTeacherAsync(teacherId);
        if (result.Succeeded) return RedirectToAction("Index", "Teachers");

        return View("Error");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        var result = await _userService.DeleteStudentAsync(studentId);
        if (result.Succeeded) return RedirectToAction("Index", "Students");

        return View("Error");
    }

    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutUserAsync();
        return RedirectToAction("Index", "Home");
    }
}