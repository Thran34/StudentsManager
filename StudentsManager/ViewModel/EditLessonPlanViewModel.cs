using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using StudentsManager.Domain.Models;

namespace StudentsManager.ViewModel;

public class EditLessonPlanViewModel
{
    public int LessonPlanId { get; set; }
    [Required] public int SelectedClassGroupId { get; set; }
    [Required] public DayOfWeek DayOfWeek { get; set; }
    public DateTime Date { get; set; }
    [Required] public Subject Subject { get; set; }
    public string? Description { get; set; }
    [Required] public int StartHour { get; set; }
    public List<SelectListItem> ClassGroups { get; set; }
}