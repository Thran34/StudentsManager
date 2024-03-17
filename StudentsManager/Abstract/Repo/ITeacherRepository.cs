using StudentsManager.Domain.Models;

namespace StudentsManager.Abstract.Repo;

public interface ITeacherRepository
{
    Task<IEnumerable<Teacher>> GetAllTeachersAsync();
    Task<Teacher?> GetTeacherByIdAsync(int teacherId);
    Task AddTeacherAsync(Teacher teacher);
    Task UpdateTeacherAsync(Teacher teacher);
    Task DeleteTeacherAsync(int teacherId);
}