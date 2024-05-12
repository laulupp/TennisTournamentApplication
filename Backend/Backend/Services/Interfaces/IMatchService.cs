using Backend.Api.Models;

namespace Backend.Services.Interfaces;

public interface IMatchService
{
    public Task<IEnumerable<MatchDTO>> GetByPlayerIdAsync(int playerId);

    public Task<IEnumerable<MatchDTO>> GetByRefereeIdAsync(int refereeId);

    public Task<IEnumerable<MatchDTO>> GetAllAsync(int? playerId, int? refereeId, int? tournamentId);

    public Task<MatchDTO> AddMatchAsync(MatchDTO m);

    public Task UpdateMatchAsync(UpdateMatchDTO m);

    public Task UpdateScoreAsync(int matchId, string score);

    public Task DeleteByIdAsync(int matchId);

    public Task DeleteByTournamentAsync(int tournamentId);

    public Task DeleteByUserAsync(int userId);
}
