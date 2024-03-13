using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _applicationDbContext;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationDbContext = applicationDbContext;
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
                        _applicationDbContext.Students.Add(student);
                    }
                    else if (model.Role == "Teacher")
                    {
                        var teacher = new Teacher
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Age = model.Age,
                            PhoneNumber = model.PhoneNumber,
                            ApplicationUserId = user.Id
                        };
                        _applicationDbContext.Teachers.Add(teacher);
                    }

                    await _applicationDbContext.SaveChangesAsync();

                    return RedirectToAction("index", "home");
                }

                foreach (var error in roleResult.Errors) ModelState.AddModelError("", error.Description);
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

            return RedirectToAction("index", "home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    public async Task<IActionResult> DeleteTeacher(int teacherId)
    {
        var teacher = await _applicationDbContext.Teachers
            .Include(t => t.ClassGroups)
            .ThenInclude(cg => cg.Students)
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId);

        if (teacher == null) return NotFound();
        var applicationUserId = teacher.ApplicationUserId;

        foreach (var classGroup in teacher.ClassGroups.ToList())
            _applicationDbContext.ClassGroups.Remove(classGroup);

        await _applicationDbContext.SaveChangesAsync();

        _applicationDbContext.Teachers.Remove(teacher);
        await _applicationDbContext.SaveChangesAsync();

        var userMessages = await _applicationDbContext.Messages
            .Where(m => m.SenderId == applicationUserId || m.ReceiverId == applicationUserId)
            .ToListAsync();

        _applicationDbContext.Messages.RemoveRange(userMessages);
        await _applicationDbContext.SaveChangesAsync();

        var applicationUser = await _userManager.FindByIdAsync(applicationUserId);
        if (applicationUser != null) await _userManager.DeleteAsync(applicationUser);

        return RedirectToAction("Index", "Teachers");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        var student = await _applicationDbContext.Students
            .Include(s => s.ApplicationUser)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (student == null) return NotFound();

        var applicationUserId = student.ApplicationUserId;

        _applicationDbContext.Students.Remove(student);
        await _applicationDbContext.SaveChangesAsync();

        var userMessages = await _applicationDbContext.Messages
            .Where(m => m.SenderId == applicationUserId || m.ReceiverId == applicationUserId)
            .ToListAsync();

        _applicationDbContext.Messages.RemoveRange(userMessages);
        await _applicationDbContext.SaveChangesAsync();

        var applicationUser = await _userManager.FindByIdAsync(applicationUserId);
        if (applicationUser != null) await _userManager.DeleteAsync(applicationUser);

        return RedirectToAction("Index", "Students");
    }


    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}