using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize]
public class ClassGroupsController : Controller
{
    private readonly Context.Context _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClassGroupsController(Context.Context context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: ClassGroups
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var isStudent = await _userManager.IsInRoleAsync(user, "Student");

        List<ClassGroup> classGroups;

        if (isAdmin)
        {
            classGroups = await _context.ClassGroups
                .Include(cg => cg.LessonPlans)
                .ToListAsync();
        }
        else if (isStudent)
        {
            var userId = _userManager.GetUserId(User);
            classGroups = await _context.ClassGroups
                .Where(cg => cg.Students.Any(s => s.ApplicationUserId == userId))
                .Include(cg => cg.LessonPlans)
                .ToListAsync();
        }

        else
        {
            var userId = _userManager.GetUserId(User);
            classGroups = await _context.ClassGroups
                .Where(cg => cg.Teacher.ApplicationUserId == userId)
                .Include(cg => cg.LessonPlans)
                .ToListAsync();
        }

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
            Name = viewModel.Name,
            TeacherId = viewModel.SelectedTeacherId
        };

        if (_context.ClassGroups.Any(x => x.Name == viewModel.Name))
        {
            ModelState.AddModelError("Name", "A class group with this name already exists.");
            return View(viewModel);
        }

        if (viewModel.SelectedStudentIds == null || !viewModel.SelectedStudentIds.Any())
        {
            ModelState.AddModelError("SelectedStudentIds", "Please select at least one student.");
            return View(viewModel);
        }

        if (viewModel.SelectedTeacherId == 0)
        {
            ModelState.AddModelError("SelectedTeacherId", "Please select a teacher.");
            return View(viewModel);
        }

        _context.ClassGroups.Add(classGroup);
        await _context.SaveChangesAsync();

        var selectedStudents = _context.Students?.Where(s => viewModel.SelectedStudentIds.Contains(s.StudentId));

        foreach (var student in selectedStudents)
            student.ClassGroupId = classGroup.ClassGroupId;

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

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var classGroup = await _context.ClassGroups
            .Include(cg => cg.Students)
            .FirstOrDefaultAsync(m => m.ClassGroupId == id);

        if (classGroup == null) return NotFound();

        foreach (var student in classGroup.Students) student.ClassGroupId = null;

        _context.ClassGroups.Remove(classGroup);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool ClassGroupExists(int id)
    {
        return _context.ClassGroups.Any(e => e.ClassGroupId == id);
    }
}