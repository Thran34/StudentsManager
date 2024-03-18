using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using StudentsManager.Domain.Models;

namespace StudentsManager.ViewModel;

public class CreateLessonPlanViewModel
{
    [Required] public int SelectedClassGroupId { get; set; }
    [Required] public DayOfWeek DayOfWeek { get; set; }
    [Required] public Subject Subject { get; set; }
    public string Description { get; set; }
    public List<SelectListItem> ClassGroups { get; set; }
    public int StartHour { get; set; }
}