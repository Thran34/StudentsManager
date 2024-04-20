using System.ComponentModel.DataAnnotations;

namespace StudentsManager.Domain.Models;

public class Teacher
{
    public int TeacherId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    [Required] public string PhoneNumber { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string ApplicationUserId { get; set; }
    public ICollection<ClassGroup> ClassGroups { get; set; }
}