namespace StudentsManager.Domain.Models;

public class LessonPlan
{
    public int LessonPlanId { get; set; }
    public int ClassGroupId { get; set; }
    public ClassGroup ClassGroup { get; set; }
    public DateTime Date { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string? Description { get; set; }
    public int StartHour { get; set; }
    public Subject Subject { get; set; }
}

public enum Subject
{
    Math,
    IT,
    PE,
    Science,
    History,
    English
}