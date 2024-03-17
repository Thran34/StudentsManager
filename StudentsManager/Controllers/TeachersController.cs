using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentsManager.Abstract.Service;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize(Roles = "Admin")]
public class TeachersController : Controller
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _teacherService.GetAllTeachersAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        return View(await _teacherService.GetTeacherByIdAsync(id));
    }

    public async Task<IActionResult> Edit(int id)
    {
        return View(await _teacherService.GetTeacherByIdAsync(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditTeacherViewModel teacherViewModel)
    {
        await _teacherService.UpdateTeacherAsync(teacherViewModel);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await _teacherService.GetTeacherByIdAsync(id);
        if (teacher == null) return NotFound();

        return View(teacher);
    }
}