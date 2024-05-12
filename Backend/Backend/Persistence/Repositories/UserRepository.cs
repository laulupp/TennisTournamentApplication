using Microsoft.EntityFrameworkCore;
using Backend.Persistence.Context;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string? username)
    {
        return await _context.Users
                             .Where(u => u.Username == username)
                             .FirstOrDefaultAsync();
    }       
    
    public async Task<User?> GetByEmailAsync(string? email)
    {
        return await _context.Users
                             .Where(u => u.Email == email)
                             .FirstOrDefaultAsync();
    }   
    

}