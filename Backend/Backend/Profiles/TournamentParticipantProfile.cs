using AutoMapper;
using Backend.Api.Models;
using Backend.Persistence.Models;

namespace Backend.Profiles;

public class TournamentParticipantProfile : Profile
{
    public TournamentParticipantProfile()
    {
        CreateMap<TournamentParticipant, TournamentParticipantDTO>().ReverseMap();
    }
}
