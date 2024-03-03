namespace StudentsManager.Models;

public class ClassGroup
{
    public int ClassGroupId { get; set; }
    public string Name { get; set; }
    public ICollection<LessonPlan> LessonPlans { get; set; }
    public ICollection<Student> Students { get; set; }
}