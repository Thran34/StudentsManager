using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentsManager.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly Context.Context _context;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager, Context.Context context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (roleResult.Succeeded)
                {
                    if (model.Role == "Student")
                    {
                        var student = new Student
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Age = model.Age,
                            PhoneNumber = model.PhoneNumber,
                            ApplicationUserId = user.Id
                        };
                        _context.Students.Add(student);
                    }
                    else if (model.Role == "Teacher")
                    {
                        var teacher = new Teacher
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Age = model.Age,
                            PhoneNumber = model.PhoneNumber,
                            Classes = model.Classes,
                            ApplicationUserId = user.Id
                        };
                        _context.Teachers.Add(teacher);
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction("index", "home");
                }
                else
                {
                    foreach (var error in roleResult.Errors) ModelState.AddModelError("", error.Description);
                }
            }

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }


    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
            false);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("index", "home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}