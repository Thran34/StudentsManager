namespace StudentsManager.Models;

public class Student : Identity
{
    public int StudentId { get; set; }
    public List<Teacher> Teachers { get; set; }
}