using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentsManager.Abstract.Service;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize(Roles = "Admin")]
public class StudentsController : Controller
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _studentService.GetAllStudentsAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        return View(await _studentService.GetStudentByIdAsync(id));
    }

    public async Task<IActionResult> Edit(int id)
    {
        return View(await _studentService.GetStudentByIdAsync(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditStudentViewModel studentViewModel)
    {
        await _studentService.UpdateStudentAsync(studentViewModel);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null) return NotFound();

        return View(student);
    }
}