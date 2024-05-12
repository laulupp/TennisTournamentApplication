using Microsoft.EntityFrameworkCore;
using Backend.Persistence.Context;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Persistence.Repositories;

public class MatchRepository : GenericRepository<Match>, IMatchRepository
{
    public MatchRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Match>> GetByPlayerIdAsync(int userId)
    {
        return await _context.Matches
                             .Where(u => u.PlayerOneId == userId || u.PlayerTwoId == userId)
                             .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetByRefereeIdAsync(int userId)
    {
        return await _context.Matches
                             .Where(u => u.RefereeId == userId)
                             .ToListAsync();
    }
}