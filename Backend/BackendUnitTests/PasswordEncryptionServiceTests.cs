using Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendUnitTests;

public class PasswordEncryptionServiceTests
{
    private readonly PasswordEncryptionService _service = new PasswordEncryptionService();

    [Fact]
    public void EncryptPasswordGivenValidPasswordShouldReturnHashedPassword()
    {
        // Arrange
        var password = "securePassword123";

        // Act
        var hashedPassword = _service.EncryptPassword(password);

        // Assert
        Assert.NotEmpty(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, hashedPassword));
    }

    [Fact]
    public void VerifyPasswordGivenValidPasswordAndHashShouldReturnTrue()
    {
        // Arrange
        var password = "securePassword123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Act
        var isValid = _service.VerifyPassword(password, hashedPassword);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPasswordGivenInvalidPasswordShouldReturnFalse()
    {
        // Arrange
        var password = "securePassword123";
        var wrongPassword = "wrongPassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Act
        var isValid = _service.VerifyPassword(wrongPassword, hashedPassword);

        // Assert
        Assert.False(isValid);
    }
}

