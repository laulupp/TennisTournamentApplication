using Backend.Api.Models;

namespace Backend.Services.Interfaces;

public interface IAuthService
{
    public Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO);

    public Task<AuthResponseDTO> RegisterAsync(RegisterDTO userDTO);
}