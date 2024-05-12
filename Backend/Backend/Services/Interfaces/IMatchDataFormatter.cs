using Backend.Api.Models;

namespace Backend.Services.Interfaces;

public interface IMatchDataFormatter
{
    string Format(IEnumerable<MatchDTO> matches);
}
