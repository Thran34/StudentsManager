using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Service;

public interface ILessonPlanService
{
    CreateLessonPlanViewModel PrepareCreateViewModelAsync(int? classGroupId, DayOfWeek? day, int? hour);
    Task<EditLessonPlanViewModel> PrepareEditViewModelAsync(int lessonPlanId);
    Task CreateLessonPlanAsync(CreateLessonPlanViewModel viewModel);
    Task UpdateLessonPlanAsync(EditLessonPlanViewModel viewModel);
    Task DeleteLessonPlanAsync(int lessonPlanId);
}