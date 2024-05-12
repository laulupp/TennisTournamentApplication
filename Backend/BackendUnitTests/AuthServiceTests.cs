using AutoMapper;
using Backend.Api.Models;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Services;
using Backend.Services.Interfaces;
using Moq;
using static Backend.Exceptions.CustomExceptions;

namespace BackendUnitTests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IPasswordEncryptionService> _passwordEncryptionServiceMock = new Mock<IPasswordEncryptionService>();
    private readonly Mock<ITokenService> _tokenServiceMock = new Mock<ITokenService>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

    private IAuthService CreateService()
    {
        return new AuthService(_userRepositoryMock.Object, _passwordEncryptionServiceMock.Object, _tokenServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task LoginAsyncUserExistsAndPasswordIsCorrectShouldReturnAuthToken()
    {
        // Arrange
        var loginDTO = new LoginDTO { Username = "testuser", Password = "testpassword" };
        var user = new User { Username = "testuser", Password = "hashedPassword" };
        var authResponse = new AuthResponseDTO { Username = "testuser", Token = "validtoken" };

        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("testuser")).ReturnsAsync(user);
        _passwordEncryptionServiceMock.Setup(p => p.VerifyPassword("testpassword", "hashedPassword")).Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateEncryptedToken("testuser")).Returns("validtoken");
        _mapperMock.Setup(m => m.Map<AuthResponseDTO>(It.IsAny<User>())).Returns(authResponse);

        var service = CreateService();

        // Act
        var result = await service.LoginAsync(loginDTO);

        // Assert
        Assert.NotNull(result.Token);
        Assert.Equal("validtoken", result.Token);
    }

    [Fact]
    public async Task LoginAsyncUserExistsAndPasswordIsIncorrectShouldThrowInvalidPasswordException()
    {
        // Arrange
        var loginDTO = new LoginDTO { Username = "testuser", Password = "wrongpassword" };
        var user = new User { Username = "testuser", Password = "hashedPassword" };

        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("testuser")).ReturnsAsync(user);
        _passwordEncryptionServiceMock.Setup(p => p.VerifyPassword("wrongpassword", "hashedPassword")).Returns(false);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPasswordException>(() => service.LoginAsync(loginDTO));
    }

    [Fact]
    public async Task RegisterAsyncUsernameAlreadyExistsShouldThrowUserAlreadyExistsException()
    {
        // Arrange
        var registerDTO = new RegisterDTO { Username = "existinguser", Password = "newpassword" };
        var existingUser = new User { Username = "existinguser", Password = "existingpassword" };

        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("existinguser")).ReturnsAsync(existingUser);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<UserAlreadyExistsException>(() => service.RegisterAsync(registerDTO));
    }

    [Fact]
    public async Task RegisterAsyncNewUserShouldReturnAuthToken()
    {
        // Arrange
        var registerDTO = new RegisterDTO { Username = "newuser", Password = "newpassword" };
        var newUser = new User { Username = "newuser", Password = "hashedNewPassword" };
        var authResponse = new AuthResponseDTO { Username = "newuser", Token = "newtoken" };

        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("newuser"))
            .Returns(Task.FromResult<User>(null));

        _userRepositoryMock.Setup(repo => repo.AddAsync(It.Is<User>(u => u.Username == "newuser" && u.Password == "hashedNewPassword")))
            .ReturnsAsync(newUser);

        _passwordEncryptionServiceMock.Setup(p => p.EncryptPassword("newpassword"))
            .Returns("hashedNewPassword");

        _tokenServiceMock.Setup(t => t.GenerateEncryptedToken("newuser"))
            .Returns("newtoken");

        _mapperMock.Setup(m => m.Map<User>(It.IsAny<RegisterDTO>()))
            .Returns((RegisterDTO source) => new User { Username = source.Username, Password = source.Password });

        _mapperMock.Setup(m => m.Map<AuthResponseDTO>(It.IsAny<User>()))
            .Returns(authResponse);

        var service = CreateService();

        // Act
        var result = await service.RegisterAsync(registerDTO);

        // Assert
        Assert.NotNull(result.Token);
        Assert.Equal("newtoken", result.Token);

        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once); // Verify that AddAsync was called
        _mapperMock.Verify(m => m.Map<AuthResponseDTO>(It.IsAny<User>()), Times.Once); // Verify that the mapping occurred
    }
}
