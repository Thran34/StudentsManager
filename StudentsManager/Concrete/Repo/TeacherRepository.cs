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

    public async Task<Teacher> FindTeacherByIdAsync(int teacherId)
    {
        return await _applicationDbContext.Teachers
            .Include(t => t.ClassGroups)
            .ThenInclude(cg => cg.Students)
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
    }

    public async Task RemoveTeacherAsync(Teacher teacher)
    {
        _applicationDbContext.Teachers.Remove(teacher);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task AddTeacherAsync(Teacher teacher)
    {
        await _applicationDbContext.Teachers.AddAsync(teacher);
        await _applicationDbContext.SaveChangesAsync();
    }
}