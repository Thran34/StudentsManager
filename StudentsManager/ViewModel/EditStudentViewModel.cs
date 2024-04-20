using Microsoft.Build.Framework;

namespace StudentsManager.ViewModel;

public class EditStudentViewModel
{
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    [Required] public string PhoneNumber { get; set; }
}