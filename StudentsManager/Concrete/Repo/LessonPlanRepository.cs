using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract.Repo;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Concrete.Repo;

public class LessonPlanRepository : ILessonPlanRepository
{
    private readonly ApplicationDbContext _context;

    public LessonPlanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LessonPlan> GetLessonPlanByIdAsync(int lessonPlanId)
    {
        return await _context.LessonPlans.FindAsync(lessonPlanId);
    }

    public async Task<List<LessonPlan>> GetAllLessonPlansAsync()
    {
        return await _context.LessonPlans.ToListAsync();
    }

    public async Task AddLessonPlanAsync(LessonPlan lessonPlan)
    {
        _context.LessonPlans.Add(lessonPlan);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLessonPlanAsync(LessonPlan lessonPlan)
    {
        _context.LessonPlans.Update(lessonPlan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLessonPlanAsync(int lessonPlanId)
    {
        var lessonPlan = await GetLessonPlanByIdAsync(lessonPlanId);
        if (lessonPlan != null)
        {
            _context.LessonPlans.Remove(lessonPlan);
            await _context.SaveChangesAsync();
        }
    }

    public CreateLessonPlanViewModel PrepareCreateViewModelAsync(int? classGroupId, DayOfWeek? day, int? hour)
    {
        return new CreateLessonPlanViewModel
        {
            ClassGroups = _context.ClassGroups.Include(cg => cg.LessonPlans)
                .Select(c => new SelectListItem { Value = c.ClassGroupId.ToString(), Text = c.Name }).ToList(),
            SelectedClassGroupId = classGroupId ?? 0,
            DayOfWeek = day ?? DayOfWeek.Monday,
            StartHour = hour ?? 8
        };
    }
}