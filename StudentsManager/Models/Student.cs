namespace StudentsManager.Models;

public class Student : Identity
{
    public List<Teacher> Teachers { get; set; }
}