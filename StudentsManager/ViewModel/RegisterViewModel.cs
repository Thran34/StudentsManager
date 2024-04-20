using System.ComponentModel.DataAnnotations;

namespace StudentsManager.ViewModel;

public class RegisterViewModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string Role { get; set; }
    [Required] public int Age { get; set; }
    [Required] public string PhoneNumber { get; set; }
}