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

public class TeacherServiceIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TeacherRepository _teacherRepository;
    private readonly IMapper _mapper;
    private readonly TeacherService _teacherService;

    public TeacherServiceIntegrationTests()
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

        _teacherRepository = new TeacherRepository(_context);

        var config = new MapperConfiguration(cfg => cfg.CreateMap<EditTeacherViewModel, Teacher>().ReverseMap());
        _mapper = config.CreateMapper();

        _teacherService = new TeacherService(_teacherRepository, _mapper);

        SetupTestData().Wait();
    }

    private async Task SetupTestData()
    {
        var teacherUser = new ApplicationUser
        {
            UserName = "teacher@example.com",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "teacher@example.com"
        };

        _context.Users.Add(teacherUser);
        await _context.SaveChangesAsync();

        var teacher = new Teacher
        {
            FirstName = "Jane",
            LastName = "Doe",
            ApplicationUserId = teacherUser.Id,
            PhoneNumber = "1234567890"
        };

        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllTeachersAsync_ShouldReturnAllTeachers()
    {
        var result = await _teacherService.GetAllTeachersAsync();

        Assert.NotEmpty(result);
        Assert.Contains(result, t => t.FirstName == "Jane" && t.LastName == "Doe");
    }

    [Fact]
    public async Task GetTeacherByIdAsync_ShouldReturnCorrectTeacher()
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync();
        Assert.NotNull(teacher);

        var result = await _teacherService.GetTeacherByIdAsync(teacher.TeacherId);

        Assert.NotNull(result);
        Assert.Equal(teacher.FirstName, result.FirstName);
        Assert.Equal(teacher.LastName, result.LastName);
        Assert.Equal(teacher.ApplicationUserId, result.ApplicationUserId);
    }

    [Fact]
    public async Task UpdateTeacherAsync_ShouldUpdateTeacherDetails()
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync();
        Assert.NotNull(teacher);

        var editTeacherViewModel = new EditTeacherViewModel
        {
            TeacherId = teacher.TeacherId,
            FirstName = "John",
            LastName = "Smith",
            PhoneNumber = "0987654321"
        };

        await _teacherService.UpdateTeacherAsync(editTeacherViewModel);

        var updatedTeacher = await _context.Teachers.FindAsync(teacher.TeacherId);
        Assert.NotNull(updatedTeacher);
        Assert.Equal("John", updatedTeacher.FirstName);
        Assert.Equal("Smith", updatedTeacher.LastName);
        Assert.Equal("0987654321", updatedTeacher.PhoneNumber);
    }

    public void Dispose()
    {
        _context.Teachers.RemoveRange(_context.Teachers);
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
        _context.Dispose();
    }
}