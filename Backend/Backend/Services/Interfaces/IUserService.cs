using Backend.Api.Models;

namespace Backend.Services.Interfaces;

public interface IUserService
{

    public Task<UserDTO> GetByIdAsync(int userId);

    public Task<UserDTO> GetByUsernameAsync(string username);

    public Task ChangePasswordAsync(string username, string oldPassword, string newPassword, bool ommitOldPasswordCheck);

    public Task UpdateInfoAsync(UserDTO userDto, string contextUsername, bool ommitUsernameCheck);

    public Task<IEnumerable<UserDTO>> GetAllUsersAsync();

    public Task<IEnumerable<UserDTO>> GetAllRefereesAsync();

    public Task<IEnumerable<UserDTO>> GetAllPlayersAsync();

    public Task DeleteByUsernameAsync(string username);
}