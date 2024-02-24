namespace StudentsManager.Models;
public class Teacher : Identity
{
    public List<Student> Students { get; set; }
}