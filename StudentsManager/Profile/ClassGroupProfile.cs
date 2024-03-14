using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Profile;

public class ClassGroupProfile : AutoMapper.Profile
{
    public ClassGroupProfile()
    {
        CreateMap<ClassGroup, CreateClassGroupViewModel>().ReverseMap();
    }
}