using StudentsManager.Domain.Models;

namespace StudentsManager.Abstract.Repo;

public interface IClassGroupRepository
{
    Task<List<ClassGroup>> GetAllClassGroupsAsync();
    Task<ClassGroup?> GetClassGroupByIdAsync(int id);
    Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id);
    Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id);
    Task CreateClassGroupAsync(ClassGroup classGroup);
    Task UpdateClassGroupAsync(ClassGroup classGroup);
    Task DeleteClassGroupAsync(ClassGroup classGroup);
    Task<bool> ClassGroupExistsAsync(int id);
}