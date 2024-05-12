using Backend.Services.Interfaces;

namespace Backend.Services;

public class PasswordEncryptionService : IPasswordEncryptionService
{
    public string EncryptPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
