using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentsManager.Abstract.Repo;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Concrete.Service;

public class ClassGroupService : IClassGroupService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IClassGroupRepository _repository;
    private readonly IMapper _mapper;

    public ClassGroupService(UserManager<ApplicationUser> userManager, IClassGroupRepository repository, IMapper mapper)
    {
        _userManager = userManager;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClassGroup>> GetAllClassGroupsAsync()
    {
        var classGroups = await _repository.GetAllClassGroupsAsync();
        return classGroups;
    }

    public async Task<ClassGroup?> GetClassGroupDetailsAsync(int id)
    {
        return await _repository.GetClassGroupByIdAsync(id);
    }

    public async Task<List<ClassGroup>> GetTeacherClassGroupsByIdAsync(string? id)
    {
        var classGroups = await _repository.GetTeacherClassGroupsByIdAsync(id);
        return classGroups;
    }

    public async Task<ClassGroup?> GetStudentClassGroupByIdAsync(string? id)
    {
        var classGroup = await _repository.GetStudentClassGroupByIdAsync(id);
        return classGroup;
    }

    public async Task<IEnumerable<ClassGroup>> GetClassGroupsForUserAsync(ApplicationUser user, string userId)
    {
        if (await _userManager.IsInRoleAsync(user, "Admin")) return await GetAllClassGroupsAsync();

        if (await _userManager.IsInRoleAsync(user, "Student"))
        {
            var classGroup = await GetStudentClassGroupByIdAsync(userId);
            return new List<ClassGroup> { classGroup };
        }

        return await GetTeacherClassGroupsByIdAsync(userId);
    }

    public async Task<CreateClassGroupViewModel> PrepareCreateClassGroupViewModelAsync()
    {
        var unassignedStudents = await _repository.GetUnassignedStudentsAsync();
        var availableTeachers = await _repository.GetAvailableTeachersAsync();

        var viewModel = new CreateClassGroupViewModel
        {
            UnassignedStudentSelectList = new SelectList(unassignedStudents, "StudentId", "FullName"),
            AvailableTeacherSelectList = new SelectList(availableTeachers, "TeacherId", "FullName")
        };

        return viewModel;
    }

    public async Task<bool> TryCreateClassGroupAsync(CreateClassGroupViewModel viewModel)
    {
        if (_repository.ClassGroupExists(viewModel.Name))
            return false;

        var classGroup = _mapper.Map<ClassGroup>(viewModel);
        await _repository.AddClassGroupAsync(classGroup, viewModel.SelectedStudentIds);

        return true;
    }

    public async Task<bool> TryDeleteClassGroupAsync(int classGroupId)
    {
        var classGroup = await _repository.GetClassGroupWithStudentsAsync(classGroupId);

        if (classGroup == null) return false;

        await _repository.DeleteClassGroupAsync(classGroup);
        return true;
    }
}