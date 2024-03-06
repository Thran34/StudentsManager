using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize(Roles = "Admin")]
public class LessonPlansController : Controller
{
    private readonly Context.Context _context;

    public LessonPlansController(Context.Context context)
    {
        _context = context;
    }

    // GET: LessonPlans/Create
    public IActionResult Create(int? classGroupId, DayOfWeek? day, int? hour)
    {
        var viewModel = new CreateLessonPlanViewModel
        {
            ClassGroups = _context.ClassGroups.Include(cg => cg.LessonPlans)
                .Select(c => new SelectListItem { Value = c.ClassGroupId.ToString(), Text = c.Name }).ToList(),
            SelectedClassGroupId = classGroupId ?? 0,
            DayOfWeek = day ?? DayOfWeek.Monday,
            StartHour = hour ?? 8
        };

        return View(viewModel);
    }

    // POST: LessonPlans/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLessonPlanViewModel viewModel)
    {
        var lessonPlan = new LessonPlan
        {
            ClassGroupId = viewModel.SelectedClassGroupId,
            DayOfWeek = viewModel.DayOfWeek,
            Subject = viewModel.Subject,
            Description = viewModel.Description,
            StartHour = viewModel.StartHour
        };

        _context.Add(lessonPlan);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "ClassGroups");
    }

    public async Task<IActionResult> Edit(int? lessonPlanId)
    {
        if (lessonPlanId == null) return NotFound();

        var lessonPlan = await _context.LessonPlans
            .Include(lp => lp.ClassGroup)
            .FirstOrDefaultAsync(lp => lp.LessonPlanId == lessonPlanId);

        if (lessonPlan == null) return NotFound();

        var viewModel = new EditLessonPlanViewModel
        {
            LessonPlanId = lessonPlan.LessonPlanId,
            SelectedClassGroupId = lessonPlan.ClassGroupId,
            DayOfWeek = lessonPlan.DayOfWeek,
            Subject = lessonPlan.Subject,
            Description = lessonPlan.Description,
            StartHour = lessonPlan.StartHour,
            ClassGroups = _context.ClassGroups.Select(c => new SelectListItem
            {
                Value = c.ClassGroupId.ToString(),
                Text = c.Name
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditLessonPlanViewModel viewModel)
    {
        var lessonPlan = await _context.LessonPlans.FindAsync(viewModel.LessonPlanId);
        if (lessonPlan == null) return NotFound();

        lessonPlan.ClassGroupId = viewModel.SelectedClassGroupId;
        lessonPlan.DayOfWeek = viewModel.DayOfWeek;
        lessonPlan.Subject = viewModel.Subject;
        lessonPlan.Description = viewModel.Description;
        lessonPlan.StartHour = viewModel.StartHour;

        try
        {
            _context.Update(lessonPlan);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.LessonPlans.Any(lp => lp.LessonPlanId == viewModel.LessonPlanId))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction("Index", "ClassGroups");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var lessonPlan = await _context.LessonPlans.FindAsync(id);
        if (lessonPlan != null)
        {
            _context.LessonPlans.Remove(lessonPlan);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Lesson plan deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Lesson plan not found.";
        }

        return RedirectToAction("Index", "ClassGroups");
    }
}