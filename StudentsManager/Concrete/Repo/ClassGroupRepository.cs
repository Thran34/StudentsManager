using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract.Repo;
using StudentsManager.Context;
using StudentsManager.Domain.Models;

namespace StudentsManager.Concrete.Repo;

public class ClassGroupRepository : IClassGroupRepository
{
    private readonly ApplicationDbContext _context;

    public ClassGroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassGroup>> GetAllClassGroupsAsync()
    {
        return await _context.ClassGroups.Include(cg => cg.LessonPlans).ToListAsync();
    }

    public async Task<ClassGroup?> GetClassGroupByIdAsync(int id)
    {
        return await _context.ClassGroups.FirstOrDefaultAsync(cg => cg.ClassGroupId == id);
    }

    public async Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id)
    {
        return await _context.ClassGroups.Include(cg => cg.LessonPlans)
            .Where(cg => cg.Students.Any(s => s.ApplicationUserId == id)).ToListAsync();
    }

    public async Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id)
    {
        return await _context.ClassGroups.Include(cg => cg.LessonPlans)
            .FirstOrDefaultAsync(cg => cg.Students.Any(s => s.ApplicationUserId == id));
    }

    public async Task CreateClassGroupAsync(ClassGroup classGroup)
    {
        _context.ClassGroups.Add(classGroup);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateClassGroupAsync(ClassGroup classGroup)
    {
        _context.ClassGroups.Update(classGroup);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteClassGroupAsync(ClassGroup classGroup)
    {
        _context.ClassGroups.Remove(classGroup);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ClassGroupExistsAsync(int id)
    {
        return await _context.ClassGroups.AnyAsync(cg => cg.ClassGroupId == id);
    }
}