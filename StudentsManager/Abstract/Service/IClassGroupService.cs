using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Service;

public interface IClassGroupService
{
    Task<ClassGroup?> GetClassGroupDetailsAsync(int id);

    Task<IEnumerable<ClassGroup>> GetClassGroupsForUserAsync(ApplicationUser user, string userId,
        DateTime? weekStartDate);

    Task<CreateClassGroupViewModel> PrepareCreateClassGroupViewModelAsync();
    Task<bool> TryCreateClassGroupAsync(CreateClassGroupViewModel viewModel);
    Task<bool> TryDeleteClassGroupAsync(int classGroupId);
}