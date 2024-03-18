using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Profile;

public class ClassGroupProfile : AutoMapper.Profile
{
    public ClassGroupProfile()
    {
        CreateMap<CreateClassGroupViewModel, ClassGroup>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.SelectedTeacherId))
            .ForMember(dest => dest.Students,
                opt => opt.Ignore());
    }
}