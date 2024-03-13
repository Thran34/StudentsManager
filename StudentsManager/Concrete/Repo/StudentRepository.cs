using StudentsManager.Abstract.Repo;
using StudentsManager.Context;
using StudentsManager.Domain.Models;

namespace StudentsManager.Concrete.Repo;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public StudentRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Student> FindStudentByIdAsync(int studentId)
    {
        return await _applicationDbContext.Students.FindAsync(studentId);
    }

    public async Task RemoveStudentAsync(Student student)
    {
        _applicationDbContext.Students.Remove(student);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task AddStudentAsync(Student student)
    {
        await _applicationDbContext.Students.AddAsync(student);
        await _applicationDbContext.SaveChangesAsync();
    }
}