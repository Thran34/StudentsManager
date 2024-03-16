using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.Models;

namespace StudentsManager.Controllers;

[Authorize(Roles = "Admin")]
public class TeachersController : Controller
{
    private readonly ApplicationDbContext _applicationDbContext;

    public TeachersController(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    // GET: Teachers
    public async Task<IActionResult> Index()
    {
        return View(await _applicationDbContext.Teachers.ToListAsync());
    }

    // GET: Teachers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var teacher = await _applicationDbContext.Teachers
            .Include(t => t.ApplicationUser)
            .Include(t => t.ClassGroups)
            .FirstOrDefaultAsync(m => m.TeacherId == id);
        if (teacher == null) return NotFound();

        return View(teacher);
    }

    // GET: Teachers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Teachers/Create
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TeacherId,FirstName,LastName,Age")] Teacher teacher)
    {
        _applicationDbContext.Add(teacher);
        await _applicationDbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Teachers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var teacher = await _applicationDbContext.Teachers.FindAsync(id);
        if (teacher == null) return NotFound();
        return View(teacher);
    }

    // POST: Teachers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("TeacherId,FirstName,LastName,Age,PhoneNumber")] Teacher teacher)
    {
        if (id != teacher.TeacherId) return NotFound();

        var teacherToUpdate = await _applicationDbContext.Teachers
            .Include(s => s.ApplicationUser)
            .Include(s => s.ClassGroups)
            .FirstOrDefaultAsync(s => s.TeacherId == id);

        if (teacherToUpdate == null) return NotFound();

        try
        {
            teacherToUpdate.FirstName = teacher.FirstName;
            teacherToUpdate.LastName = teacher.LastName;
            teacherToUpdate.Age = teacher.Age;
            teacherToUpdate.PhoneNumber = teacher.PhoneNumber;

            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeacherExists(teacherToUpdate.TeacherId))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Teachers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var teacher = await _applicationDbContext.Teachers
            .FirstOrDefaultAsync(m => m.TeacherId == id);
        if (teacher == null) return NotFound();

        return View(teacher);
    }

    // POST: Teachers/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var teacher = await _applicationDbContext.Teachers.FindAsync(id);
        _applicationDbContext.Teachers.Remove(teacher);
        await _applicationDbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TeacherExists(int id)
    {
        return _applicationDbContext.Teachers.Any(e => e.TeacherId == id);
    }
}