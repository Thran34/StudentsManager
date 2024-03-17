using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract.Repo;
using StudentsManager.Context;
using StudentsManager.Domain.Models;

namespace StudentsManager.Concrete.Repo;

public class TeacherRepository : ITeacherRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public TeacherRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        return await _applicationDbContext.Teachers
            .Include(s => s.ApplicationUser)
            .Include(s => s.ClassGroups)
            .ToListAsync();
    }

    public async Task<Teacher?> GetTeacherByIdAsync(int teacherId)
    {
        return await _applicationDbContext.Teachers
            .Include(s => s.ApplicationUser)
            .Include(t => t.ClassGroups)
            .ThenInclude(cg => cg.Students)
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
    }

    public async Task AddTeacherAsync(Teacher teacher)
    {
        await _applicationDbContext.Teachers.AddAsync(teacher);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateTeacherAsync(Teacher teacher)
    {
        _applicationDbContext.Teachers.Update(teacher);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task DeleteTeacherAsync(int teacherId)
    {
        var teacher = await _applicationDbContext.Teachers.FindAsync(teacherId);
        if (teacher != null)
        {
            _applicationDbContext.Teachers.Remove(teacher);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}