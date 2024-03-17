using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Service;

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<Student> GetStudentByIdAsync(int studentId);
    Task UpdateStudentAsync(EditStudentViewModel studentViewModel);
}