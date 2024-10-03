using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentsManager.Concrete.Repo;
using StudentsManager.Concrete.Service;
using StudentsManager.Concrete.Service.Secrets;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;
using Xunit;
using Assert = Xunit.Assert;

namespace StudentsManager.Tests.Services;

public class StudentServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly StudentRepository _studentRepository;
    private readonly IMapper _mapper;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var projectId = configuration["ProjectSettings:ProjectId"];
        var secretManager = new SecretManagerService();
        var connectionString = secretManager.GetSecretAsync("conn_string", projectId).Result;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        _studentRepository = new StudentRepository(_context);

        var config = new MapperConfiguration(cfg => cfg.CreateMap<EditStudentViewModel, Student>().ReverseMap());
        _mapper = config.CreateMapper();

        _studentService = new StudentService(_studentRepository, _mapper);

        SetupTestData().Wait();
    }

    private async Task SetupTestData()
    {
        var studentUser = new ApplicationUser
        {
            UserName = "student@example.com",
            FirstName = "John",
            LastName = "Doe",
            Email = "student@example.com"
        };

        _context.Users.Add(studentUser);
        await _context.SaveChangesAsync();

        var student = new Student
        {
            FirstName = "John",
            LastName = "Doe",
            ApplicationUserId = studentUser.Id,
            PhoneNumber = "1234567890"
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
    {
        // Act
        var result = await _studentService.GetAllStudentsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, s => s.FirstName == "John" && s.LastName == "Doe");
    }

    [Fact]
    public async Task GetStudentByIdAsync_ShouldReturnCorrectStudent()
    {
        var student = await _context.Students.FirstOrDefaultAsync();
        Assert.NotNull(student);

        var result = await _studentService.GetStudentByIdAsync(student.StudentId);

        Assert.NotNull(result);
        Assert.Equal(student.FirstName, result.FirstName);
        Assert.Equal(student.LastName, result.LastName);
        Assert.Equal(student.ApplicationUserId, result.ApplicationUserId);
    }

    [Fact]
    public async Task UpdateStudentAsync_ShouldUpdateStudentDetails()
    {
        var student = await _context.Students.FirstOrDefaultAsync();
        Assert.NotNull(student);

        var editStudentViewModel = new EditStudentViewModel
        {
            StudentId = student.StudentId,
            FirstName = "Jane",
            LastName = "Doe",
            PhoneNumber = "9876543210"
        };

        await _studentService.UpdateStudentAsync(editStudentViewModel);

        var updatedStudent = await _context.Students.FindAsync(student.StudentId);
        Assert.NotNull(updatedStudent);
        Assert.Equal("Jane", updatedStudent.FirstName);
        Assert.Equal("Doe", updatedStudent.LastName);
        Assert.Equal("9876543210", updatedStudent.PhoneNumber);
    }

    public void Dispose()
    {
        _context.Students.RemoveRange(_context.Students);
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
        _context.Dispose();
    }
}