using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StudentsManager.Concrete.Repo;
using StudentsManager.Concrete.Service;
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
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(
                "Server=34.116.143.251;Database=test;User=sqlserver;Password=1234;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        // Setting up UserManager with actual dependencies
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

        // Setting up other dependencies
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

        // Set up the common test data for use in tests
        SetupTestData().Wait();
    }

    private async Task SetupTestData()
    {
        // Create and save a teacher with a unique ID
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
        await _context.SaveChangesAsync(); // Ensure ApplicationUser is saved before adding Teacher
        await _context.Teachers.AddAsync(_teacher);
        await _context.SaveChangesAsync(); // Ensure Teacher is saved before using TeacherId

        // Verify the teacher was saved properly
        var persistedTeacher =
            await _context.Teachers.FirstOrDefaultAsync(t => t.ApplicationUserId == _teacher.ApplicationUserId);
        Assert.NotNull(persistedTeacher);
        Assert.True(persistedTeacher.TeacherId > 0, "Teacher ID should be greater than zero");

        // Create and save a student with a unique ID
        _student = new Student
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1231312132",
            ApplicationUserId = Guid.NewGuid().ToString() // Use a unique ID
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
        await _context.SaveChangesAsync(); // Ensure ApplicationUser is saved before adding Student
        await _context.Students.AddAsync(_student);
        await _context.SaveChangesAsync(); // Ensure Student is saved before using StudentId

        // Verify the student was saved properly
        var persistedStudent =
            await _context.Students.FirstOrDefaultAsync(s => s.ApplicationUserId == _student.ApplicationUserId);
        Assert.NotNull(persistedStudent);
        Assert.True(persistedStudent.StudentId > 0, "Student ID should be greater than zero");

        // Create and save a class group with the teacher and the student
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
        // Create the view model for creating a new class group
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
        // Teardown logic: Clean up all data created during the tests
        _context.ClassGroups.RemoveRange(_context.ClassGroups);
        _context.Teachers.RemoveRange(_context.Teachers);
        _context.Students.RemoveRange(_context.Students);
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();

        _context.Dispose();
    }
}