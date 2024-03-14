using AutoMapper;
using StudentsManager.Abstract.Repo;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Concrete.Service;

public class ClassGroupService : IClassGroupService
{
    private readonly IClassGroupRepository _repository;
    private readonly IMapper _mapper;

    public ClassGroupService(IClassGroupRepository repository, IMapper mapper)
    {
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

    public async Task CreateClassGroupAsync(CreateClassGroupViewModel viewModel)
    {
        var classGroup = _mapper.Map<ClassGroup>(viewModel);
        await _repository.CreateClassGroupAsync(classGroup);
    }

    public async Task UpdateClassGroupAsync(int id)
    {
        var classGroup = await _repository.GetClassGroupByIdAsync(id);
        if (classGroup != null) await _repository.UpdateClassGroupAsync(classGroup);
    }

    public async Task DeleteClassGroupAsync(int id)
    {
        var classGroup = await _repository.GetClassGroupByIdAsync(id);
        if (classGroup != null) await _repository.DeleteClassGroupAsync(classGroup);
    }
    
    
}