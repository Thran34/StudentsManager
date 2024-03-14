using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Service;

public interface IClassGroupService
{
    Task<IEnumerable<ClassGroup>> GetAllClassGroupsAsync();
    Task<ClassGroup?> GetClassGroupDetailsAsync(int id);
    Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id);
    Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id);
    Task CreateClassGroupAsync(CreateClassGroupViewModel viewModel);
    Task UpdateClassGroupAsync(int id);
    Task DeleteClassGroupAsync(int id);
}