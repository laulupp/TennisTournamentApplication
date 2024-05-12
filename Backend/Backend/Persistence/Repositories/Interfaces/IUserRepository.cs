using Backend.Persistence.Models;

namespace Backend.Persistence.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetByUsernameAsync(string? username);

    public Task<User?> GetByEmailAsync(string? email);
}
