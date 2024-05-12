using Backend.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackendUnitTests;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private TokenService _service;

    public TokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        // Setup the mock configuration to return file paths
        _configurationMock.Setup(c => c["TokenSettings:PublicKeyPath"]).Returns("..\\..\\..\\..\\Backend\\publicKey.pem");
        _configurationMock.Setup(c => c["TokenSettings:PrivateKeyPath"]).Returns("..\\..\\..\\..\\Backend\\privateKey.pem");

        _service = new TokenService(_configurationMock.Object);
    }
    [Fact]
    public void GenerateEncryptedTokenShouldReturnEncryptedToken()
    {
        // Arrange
        string username = "testUser";

        // Act
        var token = _service.GenerateEncryptedToken(username);

        // Assert
        Assert.NotNull(token);
    }

    [Fact]
    public void IsValidUserCorrectTokenShouldReturnTrue()
    {
        // Arrange
        string username = "testUser";
        var token = _service.GenerateEncryptedToken(username);

        // Act
        bool isValid = _service.IsValidUser(token, username);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidUserIncorrectTokenShouldReturnFalse()
    {
        // Arrange
        string username = "testUser";
        var token = _service.GenerateEncryptedToken(username);

        // Act
        bool isValid = _service.IsValidUser(token, "wrongUsername");

        // Assert
        Assert.False(isValid);
    }

}
