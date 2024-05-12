using Backend.Persistence.Context;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Persistence.Repositories;

public class TournamentRepository : GenericRepository<Tournament>, ITournamentRepository
{
    public TournamentRepository(AppDbContext context) : base(context) { }
}