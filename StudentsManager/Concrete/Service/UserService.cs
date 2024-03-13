using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract;
using StudentsManager.Abstract.Repo;
using StudentsManager.Abstract.Service;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Concrete.Service;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IStudentRepository _studentRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IStudentRepository studentRepository, ITeacherRepository teacherRepository,
        ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<SignInResult> LoginUserAsync(LoginViewModel model)
    {
        return await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
    }

    public async Task LogoutUserAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> DeleteTeacherAsync(int teacherId)
    {
        var teacher = await _teacherRepository.FindTeacherByIdAsync(teacherId);
        if (teacher != null)
        {
            foreach (var classGroup in teacher.ClassGroups.ToList())
                _applicationDbContext.ClassGroups.Remove(classGroup);

            var userMessages = await _applicationDbContext.Messages
                .Where(m => m.SenderId == teacher.ApplicationUserId || m.ReceiverId == teacher.ApplicationUserId)
                .ToListAsync();

            _applicationDbContext.Messages.RemoveRange(userMessages);
            await _applicationDbContext.SaveChangesAsync();

            await _teacherRepository.RemoveTeacherAsync(teacher);
            var user = await _userManager.FindByIdAsync(teacher.ApplicationUserId);
            if (user != null) return await _userManager.DeleteAsync(user);
        }

        return IdentityResult.Failed();
    }

    public async Task<IdentityResult> DeleteStudentAsync(int studentId)
    {
        var student = await _studentRepository.FindStudentByIdAsync(studentId);
        if (student != null)
        {
            var userMessages = await _applicationDbContext.Messages
                .Where(m => m.SenderId == student.ApplicationUserId || m.ReceiverId == student.ApplicationUserId)
                .ToListAsync();

            _applicationDbContext.Messages.RemoveRange(userMessages);
            await _applicationDbContext.SaveChangesAsync();

            await _studentRepository.RemoveStudentAsync(student);
            var user = await _userManager.FindByIdAsync(student.ApplicationUserId);
            if (user != null) return await _userManager.DeleteAsync(user);
        }

        return IdentityResult.Failed();
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var user = _mapper.Map<ApplicationUser>(model);

        var createUserResult = await _userManager.CreateAsync(user, model.Password);
        if (!createUserResult.Succeeded)
            return createUserResult;

        var roleAssignResult = await _userManager.AddToRoleAsync(user, model.Role);
        if (!roleAssignResult.Succeeded)
            return roleAssignResult;

        if (model.Role == "Student")
        {
            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                PhoneNumber = model.PhoneNumber,
                ApplicationUserId = user.Id
            };
            await _studentRepository.AddStudentAsync(student);
        }
        else if (model.Role == "Teacher")
        {
            var teacher = new Teacher
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                PhoneNumber = model.PhoneNumber,
                ApplicationUserId = user.Id
            };
            await _teacherRepository.AddTeacherAsync(teacher);
        }

        return createUserResult;
    }
}