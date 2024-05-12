using Backend.Persistence.Context;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Persistence.Repositories;

public class TournamentParticipantRepository : GenericRepository<TournamentParticipant>, ITournamentParticipantRepository
{
    public TournamentParticipantRepository(AppDbContext context) : base(context) { }

    public async Task<bool> IsUserWaitingForApprovalInTournament(int userId, int tournamentId)
    {
        var existingParticipant = await _context.TournamentParticipants.FirstOrDefaultAsync(tp => tp.UserId == userId && tp.TournamentId == tournamentId && !tp.Approved);

        if (existingParticipant == null)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> IsUserEnrolledInTournament(int userId, int tournamentId)
    {
        var existingParticipant = await _context.TournamentParticipants.FirstOrDefaultAsync(tp => tp.UserId == userId && tp.TournamentId == tournamentId && tp.Approved);

        if (existingParticipant == null)
        {
            return false;
        }

        return true;
    }

    public async Task ApproveLinkageAsync(TournamentParticipant linkage)
    {
        linkage.Approved = true;
        _context.TournamentParticipants.Update(linkage);

        await _context.SaveChangesAsync();
    }
}
