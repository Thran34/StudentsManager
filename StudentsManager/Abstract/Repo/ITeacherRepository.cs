using StudentsManager.Domain.Models;

namespace StudentsManager.Abstract.Repo;

public interface ITeacherRepository
{
    Task<Teacher> FindTeacherByIdAsync(int teacherId);
    Task RemoveTeacherAsync(Teacher teacher);
    Task AddTeacherAsync(Teacher teacher);
}