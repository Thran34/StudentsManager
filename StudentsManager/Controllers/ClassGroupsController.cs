using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract.Service;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize]
public class ClassGroupsController : Controller
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IClassGroupService _classGroupService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public ClassGroupsController(ApplicationDbContext applicationDbContext,
        IClassGroupService classGroupService, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _classGroupService = classGroupService;
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: ClassGroups
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var isStudent = await _userManager.IsInRoleAsync(user, "Student");

        if (isAdmin)
        {
            var classGroups = await _classGroupService.GetAllClassGroupsAsync();
            return View(classGroups);
        }
        else
        {
            var userId = _userManager.GetUserId(User);
            if (isStudent)
            {
                var classGroup = await _classGroupService.GetStudentClassGroupByIdAsync(userId);
                return View(new List<ClassGroup> { classGroup });
            }

            var classGroups = await _classGroupService.GetTeacherClassGroupsByIdAsync(userId);
            return View(classGroups);
        }
    }

    // GET: ClassGroups/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var classGroup = await _classGroupService.GetClassGroupDetailsAsync(id.Value);
        if (classGroup == null) return NotFound();

        return View(classGroup);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new CreateClassGroupViewModel
        {
            UnassignedStudentSelectList = new SelectList(
                await _applicationDbContext.Students
                    .Where(s => s.ClassGroupId == null)
                    .Select(s => new { s.StudentId, FullName = s.FirstName + " " + s.LastName })
                    .ToListAsync(),
                "StudentId",
                "FullName"),
            AvailableTeacherSelectList = new SelectList(
                await _applicationDbContext.Teachers
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

        if (_applicationDbContext.ClassGroups.Any(x => x.Name == viewModel.Name))
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

        _applicationDbContext.ClassGroups.Add(classGroup);
        await _applicationDbContext.SaveChangesAsync();

        var selectedStudents =
            _applicationDbContext.Students?.Where(s => viewModel.SelectedStudentIds.Contains(s.StudentId));

        foreach (var student in selectedStudents)
            student.ClassGroupId = classGroup.ClassGroupId;

        await _applicationDbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    // GET: ClassGroups/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var classGroup = await _applicationDbContext.ClassGroups.FindAsync(id);
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
                _applicationDbContext.Update(classGroup);
                await _applicationDbContext.SaveChangesAsync();
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

        var classGroup = await _applicationDbContext.ClassGroups
            .Include(cg => cg.Students)
            .FirstOrDefaultAsync(m => m.ClassGroupId == id);

        if (classGroup == null) return NotFound();

        foreach (var student in classGroup.Students) student.ClassGroupId = null;

        _applicationDbContext.ClassGroups.Remove(classGroup);
        await _applicationDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool ClassGroupExists(int id)
    {
        return _applicationDbContext.ClassGroups.Any(e => e.ClassGroupId == id);
    }
}