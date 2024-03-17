using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Profile;

public class TeacherProfile : AutoMapper.Profile
{
    public TeacherProfile()
    {
        CreateMap<EditTeacherViewModel, Teacher>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.ApplicationUser.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.ApplicationUser.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
    }
}