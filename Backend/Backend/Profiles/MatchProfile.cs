using AutoMapper;
using Backend.Api.Models;
using Backend.Persistence.Models;

namespace Backend.Profiles;

public class MatchProfile : Profile
{
    public MatchProfile()
    {
        CreateMap<Match, MatchDTO>().ReverseMap();
        CreateMap<Match, UpdateMatchDTO>().ReverseMap();
    }
}
