using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;
using StudentsManager.Abstract.Service;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService userService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _logger = logger;
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
        _logger.LogInformation("Attempt to register user: {Username}", model.Email);
        var result = await _userService.RegisterUserAsync(model);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Username} registered successfully", model.Email);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error registering user {Username}: {Error}", model.Email, error.Description);
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        _logger.LogInformation("Accessed Login page.");
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        var clientIp = GetClientIp();

        using (LogContext.PushProperty("ClientIp", clientIp))
        {
            _logger.LogInformation($"Attempt to login user: {model.Email}, from IP: {clientIp}");
            ViewData["ReturnUrl"] = returnUrl;

            var result = await _userService.LoginUserAsync(model);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {model.Email} logged in successfully from IP: {clientIp}.");
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }

            _logger.LogWarning($"Invalid login attempt from ip {clientIp}.");
            ModelState.AddModelError(string.Empty, $"Invalid login attempt from ip {clientIp}.");
            return View(model);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTeacher(int teacherId)
    {
        _logger.LogInformation("Attempt to delete teacher with ID: {TeacherId}", teacherId);
        var result = await _userService.DeleteTeacherAsync(teacherId);

        if (result.Succeeded)
        {
            _logger.LogInformation("Teacher with ID: {TeacherId} deleted successfully", teacherId);
            return RedirectToAction("Index", "Teachers");
        }

        _logger.LogError("Failed to delete teacher with ID: {TeacherId}", teacherId);
        return View("Error");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        _logger.LogInformation("Attempt to delete student with ID: {StudentId}", studentId);
        var result = await _userService.DeleteStudentAsync(studentId);

        if (result.Succeeded)
        {
            _logger.LogInformation("Student with ID: {StudentId} deleted successfully", studentId);
            return RedirectToAction("Index", "Students");
        }

        _logger.LogError("Failed to delete student with ID: {StudentId}", studentId);
        return View("Error");
    }

    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("User logged out.");
        await _userService.LogoutUserAsync();
        return RedirectToAction("Index", "Home");
    }

    private string GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}