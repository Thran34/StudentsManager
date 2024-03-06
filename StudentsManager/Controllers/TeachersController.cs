using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Models;

namespace StudentsManager.Controllers;

[Authorize(Roles = "Admin")]
public class TeachersController : Controller
{
    private readonly Context.Context _context;

    public TeachersController(Context.Context context)
    {
        _context = context;
    }

    // GET: Teachers
    public async Task<IActionResult> Index()
    {
        return View(await _context.Teachers.ToListAsync());
    }

    // GET: Teachers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var teacher = await _context.Teachers
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
        _context.Add(teacher);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Teachers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null) return NotFound();
        return View(teacher);
    }

    // POST: Teachers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("TeacherId,FirstName,LastName,Age")] Teacher teacher)
    {
        if (id != teacher.TeacherId) return NotFound();


        try
        {
            _context.Update(teacher);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeacherExists(teacher.TeacherId))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Teachers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var teacher = await _context.Teachers
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
        var teacher = await _context.Teachers.FindAsync(id);
        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TeacherExists(int id)
    {
        return _context.Teachers.Any(e => e.TeacherId == id);
    }
}