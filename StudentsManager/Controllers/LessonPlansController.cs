using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentsManager.Abstract.Service;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize(Roles = "Admin")]
public class LessonPlansController : Controller
{
    private readonly ILessonPlanService _lessonPlanService;

    public LessonPlansController(ILessonPlanService lessonPlanService)
    {
        _lessonPlanService = lessonPlanService;
    }

    public async Task<IActionResult> Create(int? classGroupId, DayOfWeek? day, int? hour)
    {
        var viewModel = _lessonPlanService.PrepareCreateViewModelAsync(classGroupId, day, hour);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLessonPlanViewModel viewModel)
    {
        await _lessonPlanService.CreateLessonPlanAsync(viewModel);
        return RedirectToAction("Index", "ClassGroups");
    }

    public async Task<IActionResult> Edit(int? lessonPlanId)
    {
        if (lessonPlanId == null) return NotFound();
        var viewModel = await _lessonPlanService.PrepareEditViewModelAsync(lessonPlanId.Value);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditLessonPlanViewModel viewModel)
    {
        await _lessonPlanService.UpdateLessonPlanAsync(viewModel);
        return RedirectToAction("Index", "ClassGroups");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _lessonPlanService.DeleteLessonPlanAsync(id);
        return RedirectToAction("Index", "ClassGroups");
    }
}