using AutoMapper;
using StudentsManager.Abstract.Repo;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Concrete.Service;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IMapper _mapper;

    public TeacherService(ITeacherRepository teacherRepository, IMapper mapper)
    {
        _teacherRepository = teacherRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        return _mapper.Map<IEnumerable<Teacher>>(await _teacherRepository.GetAllTeachersAsync());
    }

    public async Task<Teacher> GetTeacherByIdAsync(int teacherId)
    {
        return _mapper.Map<Teacher>(await _teacherRepository.GetTeacherByIdAsync(teacherId));
    }

    public async Task UpdateTeacherAsync(EditTeacherViewModel editTeacherViewModel)
    {
        var teacher = await _teacherRepository.GetTeacherByIdAsync(editTeacherViewModel.TeacherId);
        _mapper.Map(editTeacherViewModel, teacher);
        await _teacherRepository.UpdateTeacherAsync(teacher);
    }
}