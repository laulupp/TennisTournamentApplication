using Backend.Persistence.Models;

namespace Backend.Persistence.Repositories.Interfaces;

public interface IMatchRepository : IRepository<Match>
{
    public Task<IEnumerable<Match>> GetByPlayerIdAsync(int userId);

    public Task<IEnumerable<Match>> GetByRefereeIdAsync(int userId);
}
