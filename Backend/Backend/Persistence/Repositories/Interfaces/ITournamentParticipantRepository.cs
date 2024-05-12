using Backend.Persistence.Models;

namespace Backend.Persistence.Repositories.Interfaces;

public interface ITournamentParticipantRepository : IRepository<TournamentParticipant>
{
    public Task<bool> IsUserEnrolledInTournament(int userId, int tournamentId);

    public Task<bool> IsUserWaitingForApprovalInTournament(int userId, int tournamentId);

    public Task ApproveLinkageAsync(TournamentParticipant linkage);
}
