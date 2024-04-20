using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Service;

public interface IClassGroupService
{
    Task<IEnumerable<ClassGroup>> GetAllClassGroupsAsync(DateTime? weekStartDate);
    Task<ClassGroup?> GetClassGroupDetailsAsync(int id);
    Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id, DateTime? weekStartDate);
    Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id, DateTime? weekStartDate);

    Task<IEnumerable<ClassGroup>> GetClassGroupsForUserAsync(ApplicationUser user, string userId,
        DateTime? weekStartDate);

    Task<CreateClassGroupViewModel> PrepareCreateClassGroupViewModelAsync();
    Task<bool> TryCreateClassGroupAsync(CreateClassGroupViewModel viewModel);
    Task<bool> TryDeleteClassGroupAsync(int classGroupId);
}