using StudentsManager.Domain.Models;

namespace StudentsManager.Abstract.Repo;

public interface IClassGroupRepository
{
    Task<List<ClassGroup>> GetAllClassGroupsAsync();
    Task<ClassGroup?> GetClassGroupByIdAsync(int id);
    Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id);
    Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id);
    Task<IEnumerable<dynamic>> GetUnassignedStudentsAsync();
    Task<IEnumerable<dynamic>> GetAvailableTeachersAsync();
    bool ClassGroupExists(string name);
    Task AddClassGroupAsync(ClassGroup classGroup, IEnumerable<int> selectedStudentIds);
    Task<ClassGroup> GetClassGroupWithStudentsAsync(int classGroupId);
    Task DeleteClassGroupAsync(ClassGroup classGroup);
}