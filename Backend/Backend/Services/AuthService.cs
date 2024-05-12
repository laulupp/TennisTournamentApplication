using AutoMapper;
using Backend.Api.Models;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Persistence.Repositories;
using Backend.Services.Interfaces;
using static Backend.Exceptions.CustomExceptions;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IPasswordEncryptionService passwordEncryptionService,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordEncryptionService = passwordEncryptionService;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDTO.Username);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        if (!_passwordEncryptionService.VerifyPassword(loginDTO.Password!, user.Password!))
        {
            throw new InvalidPasswordException();
        }

        var response = _mapper.Map<AuthResponseDTO>(user);
        response.Token = _tokenService.GenerateEncryptedToken(user.Username!);

        return response;
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO userDTO)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(userDTO.Username);
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException();
        }        
        
        var existingUserByEmail = await _userRepository.GetByEmailAsync(userDTO.Username);
        if (existingUserByEmail != null)
        {
            throw new EmailAlreadyRegisteredException();
        }

        var user = _mapper.Map<User>(userDTO);
        user.Password = _passwordEncryptionService.EncryptPassword(userDTO.Password!);

        await _userRepository.AddAsync(user);

        var response = _mapper.Map<AuthResponseDTO>(user);
        response.Token = _tokenService.GenerateEncryptedToken(user.Username!);

        return response;
    }
}