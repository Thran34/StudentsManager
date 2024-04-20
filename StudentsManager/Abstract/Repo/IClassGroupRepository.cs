using StudentsManager.Domain.Models;

namespace StudentsManager.Abstract.Repo;

public interface IClassGroupRepository
{
    Task<List<ClassGroup>> GetAllClassGroupsAsync(DateTime? weekStartDate);
    Task<ClassGroup?> GetClassGroupByIdAsync(int id);
    Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id, DateTime? weekStartDate);
    Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id, DateTime? weekStartDate);
    Task<IEnumerable<dynamic>> GetUnassignedStudentsAsync();
    Task<IEnumerable<dynamic>> GetAvailableTeachersAsync();
    bool ClassGroupExists(string name);
    Task AddClassGroupAsync(ClassGroup classGroup, IEnumerable<int> selectedStudentIds);
    Task<ClassGroup> GetClassGroupWithStudentsAsync(int classGroupId);
    Task DeleteClassGroupAsync(ClassGroup classGroup);
}