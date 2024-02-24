namespace StudentsManager.Models;
public class Teacher : Identity
{
    public int TeacherId { get; set; }
    public List<Student> Students { get; set; }
}