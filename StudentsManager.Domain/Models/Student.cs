namespace StudentsManager.Domain.Models;

public class Student
{
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Age { get; set; }
    public string PhoneNumber { get; set; }
    public int? ClassGroupId { get; set; }
    public ClassGroup? ClassGroup { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string ApplicationUserId { get; set; }
}