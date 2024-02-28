namespace StudentsManager.Models;

public class Teacher
{
    public int TeacherId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Age { get; set; }
    public string PhoneNumber { get; set; }
    public Classes Classes { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string ApplicationUserId { get; set; }
}

public enum Classes
{
    Math,
    IT,
    PE,
    Science,
    History,
    English
}