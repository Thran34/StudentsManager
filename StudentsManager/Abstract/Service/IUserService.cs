using Microsoft.AspNetCore.Identity;
using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Service;

public interface IUserService
{
    Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
    Task<SignInResult> LoginUserAsync(LoginViewModel model);
    Task LogoutUserAsync();
    Task<IdentityResult> DeleteTeacherAsync(int teacherId);
    Task<IdentityResult> DeleteStudentAsync(int studentId);
}