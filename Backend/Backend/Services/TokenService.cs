using Backend.Services.Interfaces;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

namespace Backend.Services;

public class TokenService : ITokenService
{
    private readonly RSA _publicKey;
    private readonly RSA _privateKey;

    public TokenService(IConfiguration configuration)
    {
        string publicKeyPath = configuration["TokenSettings:PublicKeyPath"];
        string privateKeyPath = configuration["TokenSettings:PrivateKeyPath"];
        _publicKey = LoadPublicKey(publicKeyPath);
        _privateKey = LoadPrivateKey(privateKeyPath);
    }

    private static RSA LoadPublicKey(string path)
    {
        string publicKeyPem = File.ReadAllText(path);

        RSA rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem.ToCharArray());
        return rsa;
    }

    private static RSA LoadPrivateKey(string path)
    {
        string privateKeyPem = File.ReadAllText(path);
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem.ToCharArray());
        return rsa;
    }

    public string GenerateEncryptedToken(string username)
    {
        string jsonToken = JsonSerializer.Serialize(username);
        byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(jsonToken);

        byte[] encryptedBytes = _publicKey.Encrypt(bytesToEncrypt, RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(encryptedBytes);
    }

    public bool IsValidUser(string? token, string? username)
    {
        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(token);
            byte[] decryptedBytes = _privateKey.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            string decryptedToken = Encoding.UTF8.GetString(decryptedBytes);
            string decryptedUsername = JsonSerializer.Deserialize<string>(decryptedToken);

            return decryptedUsername == username;
        }
        catch
        {
            return false;
        }
    }
}