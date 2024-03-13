using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.Models;

namespace StudentsManager.Controllers;

[Authorize(Roles = "Admin")]
public class StudentsController : Controller
{
    private readonly ApplicationDbContext _applicationDbContext;

    public StudentsController(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    // GET: Students
    public async Task<IActionResult> Index()
    {
        return _applicationDbContext.Students != null
            ? View(await _applicationDbContext.Students.ToListAsync())
            : Problem("Entity set 'ApplicationDbContext.Students'  is null.");
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _applicationDbContext.Students == null) return NotFound();

        var student = await _applicationDbContext.Students
            .FirstOrDefaultAsync(m => m.StudentId == id);
        if (student == null) return NotFound();

        return View(student);
    }

    // GET: Students/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("StudentId,FirstName,LastName,Age")] Student student)
    {
        _applicationDbContext.Students.Add(student);
        await _applicationDbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _applicationDbContext.Students == null) return NotFound();

        var student = await _applicationDbContext.Students.FindAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    // POST: Students/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("StudentId,FirstName,LastName,Age,Teachers")] Student student)
    {
        if (id != student.StudentId) return NotFound();

        try
        {
            _applicationDbContext.Update(student);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(student.StudentId))
                return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));

        return View(student);
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _applicationDbContext.Students == null) return NotFound();

        var student = await _applicationDbContext.Students
            .FirstOrDefaultAsync(m => m.StudentId == id);
        if (student == null) return NotFound();

        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_applicationDbContext.Students == null)
            return Problem("Entity set 'ApplicationDbContext.Students' is null.");
        var student = await _applicationDbContext.Students.FindAsync(id);
        if (student != null) _applicationDbContext.Students.Remove(student);

        await _applicationDbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int id)
    {
        return (_applicationDbContext.Students?.Any(e => e.StudentId == id)).GetValueOrDefault();
    }
}