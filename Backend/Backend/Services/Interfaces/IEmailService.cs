namespace Backend.Services.Interfaces;

public interface IEmailService
{
    void SendMail(string email, string subject, string message);
}