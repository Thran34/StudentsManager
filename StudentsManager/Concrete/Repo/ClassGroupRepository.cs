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

    public async Task<List<ClassGroup>> GetAllClassGroupsAsync(DateTime? weekStartDate)
    {
        var weekEnd = weekStartDate?.AddDays(6);
        return await _context.ClassGroups
            .Include(cg => cg.LessonPlans.Where(x => x.Date >= weekStartDate && x.Date <= weekEnd)).ToListAsync();
    }

    public async Task<ClassGroup?> GetClassGroupByIdAsync(int id)
    {
        return await _context.ClassGroups.FirstOrDefaultAsync(cg => cg.ClassGroupId == id);
    }

    public async Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id, DateTime? weekStartDate)
    {
        var weekEnd = weekStartDate?.AddDays(6);
        return await _context.ClassGroups
            .Include(cg => cg.LessonPlans.Where(x => x.Date >= weekStartDate && x.Date <= weekEnd))
            .Where(cg => cg.Teacher.ApplicationUserId == id).ToListAsync();
    }

    public async Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id, DateTime? weekStartDate)
    {
        var weekEnd = weekStartDate?.AddDays(6);

        return await _context.ClassGroups
            .Include(cg => cg.LessonPlans.Where(x => x.Date >= weekStartDate && x.Date <= weekEnd))
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

    public async Task<bool> ClassGroupExistsAsync(int id)
    {
        return await _context.ClassGroups.AnyAsync(cg => cg.ClassGroupId == id);
    }

    public async Task<IEnumerable<dynamic>> GetUnassignedStudentsAsync()
    {
        return await _context.Students
            .Where(s => s.ClassGroupId == null)
            .Select(s => new { s.StudentId, FullName = s.FirstName + " " + s.LastName })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetAvailableTeachersAsync()
    {
        return await _context.Teachers
            .Select(t => new { t.TeacherId, FullName = t.FirstName + " " + t.LastName })
            .ToListAsync();
    }

    public bool ClassGroupExists(string name)
    {
        return _context.ClassGroups.Any(cg => cg.Name == name);
    }

    public async Task AddClassGroupAsync(ClassGroup classGroup, IEnumerable<int> selectedStudentIds)
    {
        _context.ClassGroups.Add(classGroup);
        await _context.SaveChangesAsync();

        var students = _context.Students.Where(s => selectedStudentIds.Contains(s.StudentId));
        foreach (var student in students) student.ClassGroupId = classGroup.ClassGroupId;

        await _context.SaveChangesAsync();
    }

    public async Task<ClassGroup> GetClassGroupWithStudentsAsync(int classGroupId)
    {
        return await _context.ClassGroups
            .Include(cg => cg.Students)
            .FirstOrDefaultAsync(m => m.ClassGroupId == classGroupId);
    }

    public async Task DeleteClassGroupAsync(ClassGroup classGroup)
    {
        foreach (var student in
                 classGroup.Students) student.ClassGroupId = null; // Unassign students from the class group

        _context.ClassGroups.Remove(classGroup);
        await _context.SaveChangesAsync();
    }
}