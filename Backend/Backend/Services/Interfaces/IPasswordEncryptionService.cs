namespace Backend.Services.Interfaces;

public interface IPasswordEncryptionService
{
    string EncryptPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
