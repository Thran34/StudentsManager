using StudentsManager.Domain.Models;

namespace StudentsManager.Abstract.Repo;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentByIdAsync(int studentId);
    Task AddStudentAsync(Student student);
    Task UpdateStudentAsync(Student student);
    Task DeleteStudentAsync(int studentId);
}