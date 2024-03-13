using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Profile;

public class ApplicationMappingProfile : AutoMapper.Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<RegisterViewModel, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
    }
}