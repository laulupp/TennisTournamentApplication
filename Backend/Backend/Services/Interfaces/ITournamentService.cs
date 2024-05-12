using Backend.Api.Models;

namespace Backend.Services.Interfaces;


public interface ITournamentService
{
    public Task<IEnumerable<TournamentDTO>> GetAllAsync(int userId);

    public Task<TournamentDTO?> GetTournamentByIdAsync(int tournamentId);

    public Task<TournamentDTO> AddTournamentAsync(TournamentDTO dto);

    public Task UpdateTournamentAsync(TournamentDTO dto);

    public Task AddUserToTournamentAsync(int userId, int tournamentId);

    public Task<IEnumerable<int>> GetUserIdsByTournamentAsync(int tournamentId);

    public Task<IEnumerable<TournamentParticipantDTO>> GetPendingTournamentRequestsAsync();

    public Task DeleteUsersFromTournamentAsync(int? tournamentId = null, int? userId = null);

    public Task DeleteAsync(int tournamentId);

    public Task ApproveRequestAsync(int tournamentId, int userId);

    public Task DenyRequestAsync(int tournamentId, int userId);
}

