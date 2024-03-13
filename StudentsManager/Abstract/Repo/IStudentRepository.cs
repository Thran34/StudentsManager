using StudentsManager.Domain.Models;

namespace StudentsManager.Abstract.Repo;

public interface IStudentRepository
{
    Task<Student> FindStudentByIdAsync(int studentId);
    Task RemoveStudentAsync(Student student);
    Task AddStudentAsync(Student student);
}