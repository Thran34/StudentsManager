using AutoMapper;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

public class LessonPlanProfile : Profile
{
    public LessonPlanProfile()
    {
        CreateMap<CreateLessonPlanViewModel, LessonPlan>()
            .ForMember(dest => dest.ClassGroupId, opt => opt.MapFrom(src => src.SelectedClassGroupId))
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.StartHour, opt => opt.MapFrom(src => src.StartHour))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject));

        CreateMap<LessonPlan, EditLessonPlanViewModel>()
            .ForMember(dest => dest.LessonPlanId, opt => opt.MapFrom(src => src.LessonPlanId))
            .ForMember(dest => dest.SelectedClassGroupId, opt => opt.MapFrom(src => src.ClassGroupId))
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.StartHour, opt => opt.MapFrom(src => src.StartHour))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject));

        CreateMap<EditLessonPlanViewModel, LessonPlan>()
            .ForMember(dest => dest.ClassGroupId, opt => opt.MapFrom(src => src.SelectedClassGroupId))
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.StartHour, opt => opt.MapFrom(src => src.StartHour))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.LessonPlanId, opt => opt.MapFrom(src => src.LessonPlanId));
    }
}