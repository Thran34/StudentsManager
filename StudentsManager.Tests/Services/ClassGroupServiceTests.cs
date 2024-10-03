using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StudentsManager.Concrete.Repo;
using StudentsManager.Concrete.Service;
using StudentsManager.Concrete.Service.Secrets;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;
using Xunit;
using Assert = Xunit.Assert;

namespace StudentsManager.Tests.Services;

public class ClassGroupServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ClassGroupRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ClassGroupService _service;

    // Common test data
    private Teacher _teacher;
    private Student _student;
    private ClassGroup _classGroup;

    public ClassGroupServiceTests()
    {
        var secretManager = new SecretManagerService();
        var connectionString = secretManager.GetSecretAsync("conn_string", "aj-dev-434320").Result;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        var store = new UserStore<ApplicationUser>(_context);
        _userManager = new UserManager<ApplicationUser>(
            store,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            new IUserValidator<ApplicationUser>[0],
            new IPasswordValidator<ApplicationUser>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object
        );

        _repository = new ClassGroupRepository(_context);
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateClassGroupViewModel, ClassGroup>()
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.SelectedTeacherId))
                .ForMember(dest => dest.Students,
                    opt => opt.Ignore());
        });
        _mapper = config.CreateMapper();
        _mapper = config.CreateMapper();

        _service = new ClassGroupService(_userManager, _repository, _mapper);

        SetupTestData().Wait();
    }

    private async Task SetupTestData()
    {
        _teacher = new Teacher
        {
            FirstName = "Jane",
            LastName = "Doe",
            PhoneNumber = "1231312132",
            ApplicationUserId = Guid.NewGuid().ToString() // Use a unique ID
        };
        var teacherUser = new ApplicationUser
        {
            Id = _teacher.ApplicationUserId,
            UserName = "teacher@example.com",
            FirstName = "John",
            LastName = "Doe",
            Email = "teacher@example.com"
        };
        _context.Users.Add(teacherUser);
        await _context.SaveChangesAsync();
        await _context.Teachers.AddAsync(_teacher);
        await _context.SaveChangesAsync();

        var persistedTeacher =
            await _context.Teachers.FirstOrDefaultAsync(t => t.ApplicationUserId == _teacher.ApplicationUserId);
        Assert.NotNull(persistedTeacher);
        Assert.True(persistedTeacher.TeacherId > 0, "Teacher ID should be greater than zero");

        _student = new Student
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1231312132",
            ApplicationUserId = Guid.NewGuid().ToString()
        };
        var studentUser = new ApplicationUser
        {
            Id = _student.ApplicationUserId,
            UserName = "student@example.com",
            FirstName = "John",
            LastName = "Doe",
            Email = "student@example.com"
        };
        _context.Users.Add(studentUser);
        await _context.SaveChangesAsync();
        await _context.Students.AddAsync(_student);
        await _context.SaveChangesAsync();

        var persistedStudent =
            await _context.Students.FirstOrDefaultAsync(s => s.ApplicationUserId == _student.ApplicationUserId);
        Assert.NotNull(persistedStudent);
        Assert.True(persistedStudent.StudentId > 0, "Student ID should be greater than zero");

        _classGroup = new ClassGroup
        {
            Name = "Class A",
            TeacherId = persistedTeacher.TeacherId,
            Students = new List<Student> { persistedStudent }
        };
        await _context.ClassGroups.AddAsync(_classGroup);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllClassGroupsAsync_ShouldReturnClassGroups()
    {
        // Act
        var result = await _service.GetAllClassGroupsAsync(null);

        // Assert
        Assert.Contains(result, cg => cg.Name == _classGroup.Name);
    }

    [Fact]
    public async Task TryCreateClassGroupAsync_ShouldReturnFalse_WhenClassGroupExists()
    {
        var viewModel = new CreateClassGroupViewModel { Name = "Class A" };
        var result = await _service.TryCreateClassGroupAsync(viewModel);

        Assert.False(result);
    }

    [Fact]
    public async Task TryCreateClassGroupAsync_ShouldReturnTrue_WhenClassGroupDoesNotExist()
    {
        var viewModel = new CreateClassGroupViewModel
        {
            Name = "Class B",
            SelectedTeacherId = _teacher.TeacherId,
            SelectedStudentIds = new[] { _student.StudentId }
        };
        // Act
        var result = await _service.TryCreateClassGroupAsync(viewModel);

        // Assert
        Assert.True(result);
    }

    public void Dispose()
    {
        _context.ClassGroups.RemoveRange(_context.ClassGroups);
        _context.Teachers.RemoveRange(_context.Teachers);
        _context.Students.RemoveRange(_context.Students);
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();

        _context.Dispose();
    }
}