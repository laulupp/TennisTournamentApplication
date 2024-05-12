namespace Backend.Services.Interfaces;

public interface ITokenService
{
    string GenerateEncryptedToken(string username);
    bool IsValidUser(string? token, string? username);
}
