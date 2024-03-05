namespace StudentsManager.Models;

public class LessonPlan
{
    public int LessonPlanId { get; set; }
    public int ClassGroupId { get; set; }
    public ClassGroup ClassGroup { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public int StartHour { get; set; }
}