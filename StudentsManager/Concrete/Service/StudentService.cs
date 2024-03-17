using AutoMapper;
using StudentsManager.Abstract.Repo;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Concrete.Service;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IMapper _mapper;

    public StudentService(IStudentRepository studentRepository, IMapper mapper)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return _mapper.Map<IEnumerable<Student>>(await _studentRepository.GetAllStudentsAsync());
    }

    public async Task<Student> GetStudentByIdAsync(int studentId)
    {
        return _mapper.Map<Student>(await _studentRepository.GetStudentByIdAsync(studentId));
    }

    public async Task UpdateStudentAsync(EditStudentViewModel editStudentViewModel)
    {
        var student = await _studentRepository.GetStudentByIdAsync(editStudentViewModel.StudentId);
        _mapper.Map(editStudentViewModel, student);
        await _studentRepository.UpdateStudentAsync(student);
    }
}