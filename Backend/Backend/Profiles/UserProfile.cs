using Backend.Api.Models;
using Backend.Persistence.Models;
using AutoMapper;

namespace Backend.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>().ForMember(dest => dest.Password, opts => opts.Ignore());
        CreateMap<UserDTO, User>().ForMember(dest => dest.Id, opts => opts.Ignore())
                                  .ForMember(dest => dest.Role, opts => opts.Ignore())
                                  .ForMember(dest => dest.Password, opts => opts.Ignore());
        CreateMap<User, AuthResponseDTO>();
        CreateMap<RegisterDTO, User>().ForMember(dest => dest.Role, opts => opts.MapFrom((s) => s.IsReferee ? 1 : 0));
    }
}
