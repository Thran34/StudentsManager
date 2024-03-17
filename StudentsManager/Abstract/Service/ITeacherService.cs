using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Service;

public interface ITeacherService
{
    Task<IEnumerable<Teacher>> GetAllTeachersAsync();
    Task<Teacher> GetTeacherByIdAsync(int studentId);
    Task UpdateTeacherAsync(EditTeacherViewModel teacherViewModel);
}