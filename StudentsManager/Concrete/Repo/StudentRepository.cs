using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract.Repo;
using StudentsManager.Context;
using StudentsManager.Domain.Models;

namespace StudentsManager.Concrete.Repo;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _context.Students
            .Include(s => s.ApplicationUser)
            .Include(s => s.ClassGroup)
            .ToListAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
            .Include(s => s.ApplicationUser)
            .Include(s => s.ClassGroup)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task AddStudentAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStudentAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStudentAsync(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}