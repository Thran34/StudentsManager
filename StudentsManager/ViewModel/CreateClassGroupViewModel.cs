using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace StudentsManager.ViewModel;

public class CreateClassGroupViewModel
{
    [Required] public string Name { get; set; }
    public List<SelectListItem> UnassignedStudents { get; set; } = new();
    public List<SelectListItem> AvailableTeachers { get; set; } = new();
    public SelectList UnassignedStudentSelectList { get; set; }
    public SelectList AvailableTeacherSelectList { get; set; }
    public int[] SelectedStudentIds { get; set; }
    public int SelectedTeacherId { get; set; }
}