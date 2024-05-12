using Backend.Persistence.Models;
using Backend.Api.Models;
using AutoMapper;
using Backend.Services.Interfaces;
using static Backend.Exceptions.CustomExceptions;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordEncryptionService _passwordEncryptionService;

    public UserService(IUserRepository userRepository, IMapper mapper, IPasswordEncryptionService passwordEncryptionService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordEncryptionService = passwordEncryptionService;
    }

    public async Task<UserDTO> GetByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        return _mapper.Map<User, UserDTO>(user);
    }    
    
    public async Task<UserDTO> GetByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        return _mapper.Map<UserDTO>(user);
    }

    public async Task ChangePasswordAsync(string username, string oldPassword, string newPassword, bool ommitOldPasswordCheck = false)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if(user == null)
        {
            throw new UserNotFoundException();
        }

        if(!ommitOldPasswordCheck && !_passwordEncryptionService.VerifyPassword(oldPassword, user.Password!))
        {
            throw new InvalidPasswordException();
        }

        user.Password = _passwordEncryptionService.EncryptPassword(newPassword);
        await _userRepository.UpdateAsync(user);
    }

    public async Task UpdateInfoAsync(UserDTO userDto, string contextUsername, bool ommitUsernameCheck = false)
    {
        if (!ommitUsernameCheck && userDto.Username != contextUsername)
        {
            throw new UserUnauthorizedException();
        }

        var user = await _userRepository.GetByUsernameAsync(userDto.Username);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        //Check if email was changed. If changed, check if such an email is already registered

        var userByEmail = await _userRepository.GetByEmailAsync(userDto.Email);
        if(userByEmail != null && userByEmail.Username != userDto.Username) 
        {
            throw new EmailAlreadyRegisteredException();
        }

        if (!ommitUsernameCheck && (userDto.Password == null  || !_passwordEncryptionService.VerifyPassword(userDto.Password, user.Password!)))
        {
            throw new InvalidPasswordException();
        }

        _mapper.Map(userDto, user);

        await _userRepository.UpdateAsync(user);
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDTO>>(users);
    }

    public async Task<IEnumerable<UserDTO>> GetAllRefereesAsync()
    {
        var users = await _userRepository.GetAllAsync();
        users = users.Where(u => u.Role == Role.Referee);

        return _mapper.Map<IEnumerable<UserDTO>>(users);
    }

    public async Task<IEnumerable<UserDTO>> GetAllPlayersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        users = users.Where(u => u.Role == Role.Player);

        return _mapper.Map<IEnumerable<UserDTO>>(users);
    }

    public async Task DeleteByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        await _userRepository.DeleteAsync(user.Id);
    }
}