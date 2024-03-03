using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize(Roles = "Admin,Teacher")]
public class ClassGroupsController : Controller
{
    private readonly Context.Context _context;

    public ClassGroupsController(Context.Context context)
    {
        _context = context;
    }

    // GET: ClassGroups
    public async Task<IActionResult> Index()
    {
        var classGroups = await _context.ClassGroups.Include(cg => cg.LessonPlans).ToListAsync();
        return View(classGroups);
    }

    // GET: ClassGroups/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var classGroup = await _context.ClassGroups
            .Include(cg => cg.LessonPlans)
            .FirstOrDefaultAsync(m => m.ClassGroupId == id);

        if (classGroup == null) return NotFound();

        return View(classGroup);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new CreateClassGroupViewModel
        {
            UnassignedStudentSelectList = new SelectList(
                await _context.Students
                    .Where(s => s.ClassGroupId == null)
                    .Select(s => new { s.StudentId, FullName = s.FirstName + " " + s.LastName })
                    .ToListAsync(),
                "StudentId",
                "FullName"),
            AvailableTeacherSelectList = new SelectList(
                await _context.Teachers
                    .Select(t => new
                    {
                        t.TeacherId,
                        FullName = t.FirstName + " " + t.LastName
                    })
                    .ToListAsync(),
                "TeacherId",
                "FullName")
        };

        return View(viewModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClassGroupViewModel viewModel)
    {
        var classGroup = new ClassGroup
        {
            Name = viewModel.Name
            // Initialize other properties as necessary
        };

        _context.ClassGroups.Add(classGroup);
        await _context.SaveChangesAsync();

        // Assign selected students to the class group
        var selectedStudents = _context.Students.Where(s => viewModel.SelectedStudentIds.Contains(s.StudentId));
        foreach (var student in
                 selectedStudents)
            student.ClassGroupId = classGroup.ClassGroupId; // Assuming you have a navigation property

        // Assign selected teacher to the class group
        // Depending on your model, this might vary

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // GET: ClassGroups/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var classGroup = await _context.ClassGroups.FindAsync(id);
        if (classGroup == null) return NotFound();

        return View(classGroup);
    }

// POST: ClassGroups/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ClassGroupId,Name")] ClassGroup classGroup)
    {
        if (id != classGroup.ClassGroupId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(classGroup);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassGroupExists(classGroup.ClassGroupId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(classGroup);
    }

    private bool ClassGroupExists(int id)
    {
        return _context.ClassGroups.Any(e => e.ClassGroupId == id);
    }
}