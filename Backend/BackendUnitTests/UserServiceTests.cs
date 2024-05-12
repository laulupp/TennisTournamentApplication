using AutoMapper;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Api.Models;
using Backend.Persistence.Models;
using static Backend.Exceptions.CustomExceptions;

namespace BackendUnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly Mock<IPasswordEncryptionService> _passwordEncryptionServiceMock = new Mock<IPasswordEncryptionService>();
    private UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_userRepositoryMock.Object, _mapperMock.Object, _passwordEncryptionServiceMock.Object);
    }

    [Fact]
    public async Task GetByIdAsyncUserExistsReturnsUserDTO()
    {
        // Arrange
        var user = new User { Id = 1, Username = "testUser" };
        var userDTO = new UserDTO { Id = 1, Username = "testUser" };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<User, UserDTO>(user)).Returns(userDTO);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testUser", result.Username);
    }

    [Fact]
    public async Task GetByIdAsync_UserNotExists_ThrowsUserNotFoundException()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidCredentials_ChangesPassword()
    {
        // Arrange
        var user = new User { Id = 1, Username = "user1", Password = "hashedOldPassword" };
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync("user1")).ReturnsAsync(user);
        _passwordEncryptionServiceMock.Setup(x => x.VerifyPassword("oldPassword", "hashedOldPassword")).Returns(true);
        _passwordEncryptionServiceMock.Setup(x => x.EncryptPassword("newPassword")).Returns("hashedNewPassword");
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);


        // Act
        await _service.ChangePasswordAsync("user1", "oldPassword", "newPassword", false);

        // Assert
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Password == "hashedNewPassword")), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_InvalidOldPassword_ThrowsInvalidPasswordException()
    {
        // Arrange
        var user = new User { Id = 1, Username = "user1", Password = "hashedOldPassword" };
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync("user1")).ReturnsAsync(user);
        _passwordEncryptionServiceMock.Setup(x => x.VerifyPassword("wrongOldPassword", "hashedOldPassword")).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPasswordException>(() => _service.ChangePasswordAsync("user1", "wrongOldPassword", "newPassword", false));
    }

}
