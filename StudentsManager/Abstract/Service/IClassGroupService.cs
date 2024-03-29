using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Service;

public interface IClassGroupService
{
    Task<IEnumerable<ClassGroup>> GetAllClassGroupsAsync();
    Task<ClassGroup?> GetClassGroupDetailsAsync(int id);
    Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id);
    Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id);
    Task<IEnumerable<ClassGroup>> GetClassGroupsForUserAsync(ApplicationUser user, string userId);
    Task<CreateClassGroupViewModel> PrepareCreateClassGroupViewModelAsync();
    Task<bool> TryCreateClassGroupAsync(CreateClassGroupViewModel viewModel);
    Task<bool> TryDeleteClassGroupAsync(int classGroupId);
}