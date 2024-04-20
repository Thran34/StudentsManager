using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentsManager.Abstract.Repo;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Concrete.Service;

public class LessonPlanService : ILessonPlanService
{
    private readonly ILessonPlanRepository _lessonPlanRepository;
    private readonly IClassGroupRepository _classGroupRepository;
    private readonly IMapper _mapper;

    public LessonPlanService(ILessonPlanRepository lessonPlanRepository, IClassGroupRepository classGroupRepository,
        IMapper mapper)
    {
        _lessonPlanRepository = lessonPlanRepository;
        _classGroupRepository = classGroupRepository;
        _mapper = mapper;
    }

    public CreateLessonPlanViewModel PrepareCreateViewModelAsync(int? classGroupId, DayOfWeek? day, int? hour,
        DateTime? date)
    {
        return _lessonPlanRepository.PrepareCreateViewModelAsync(classGroupId, day, hour, date);
    }

    public async Task<EditLessonPlanViewModel> PrepareEditViewModelAsync(int lessonPlanId)
    {
        var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(lessonPlanId);
        var viewModel = _mapper.Map<EditLessonPlanViewModel>(lessonPlan);

        viewModel.ClassGroups = _classGroupRepository.GetAllClassGroupsAsync(null).Result.Select(c =>
            new SelectListItem
            {
                Value = c.ClassGroupId.ToString(),
                Text = c.Name
            }).ToList();
        return viewModel;
    }

    public async Task CreateLessonPlanAsync(CreateLessonPlanViewModel viewModel)
    {
        var lessonPlan = _mapper.Map<LessonPlan>(viewModel);
        await _lessonPlanRepository.AddLessonPlanAsync(lessonPlan);
    }

    public async Task UpdateLessonPlanAsync(EditLessonPlanViewModel viewModel)
    {
        var lessonPlan = _mapper.Map<LessonPlan>(viewModel);
        await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan);
    }

    public async Task DeleteLessonPlanAsync(int lessonPlanId)
    {
        await _lessonPlanRepository.DeleteLessonPlanAsync(lessonPlanId);
    }
}