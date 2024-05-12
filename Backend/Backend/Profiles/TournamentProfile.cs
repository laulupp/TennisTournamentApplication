using AutoMapper;
using Backend.Api.Models;
using Backend.Persistence.Models;

namespace Backend.Profiles;

public class TournamentProfile : Profile
{
    public TournamentProfile()
    {
        CreateMap<Tournament, TournamentDTO>().ReverseMap();
    }
}
