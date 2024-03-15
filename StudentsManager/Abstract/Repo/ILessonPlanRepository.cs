using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Abstract.Repo;

public interface ILessonPlanRepository
{
    Task<LessonPlan> GetLessonPlanByIdAsync(int lessonPlanId);
    Task<List<LessonPlan>> GetAllLessonPlansAsync();
    Task AddLessonPlanAsync(LessonPlan lessonPlan);
    Task UpdateLessonPlanAsync(LessonPlan lessonPlan);
    Task DeleteLessonPlanAsync(int lessonPlanId);
    CreateLessonPlanViewModel PrepareCreateViewModelAsync(int? classGroupId, DayOfWeek? day, int? hour);
}