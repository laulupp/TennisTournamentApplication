using Backend.Services.Interfaces;
using System.Net.Mail;
using System.Net;
namespace Backend.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _apiKey;
    private readonly string _apiSecret;

    public EmailService(string smtpServer, int smtpPort, string apiKey, string apiSecret)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _apiKey = apiKey;
        _apiSecret = apiSecret;
    }

    public void SendMail(string email, string subject, string message)
    {
        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("tennistournamentproject@gmail.com");
        mailMessage.To.Add(email);
        mailMessage.Body = message;
        mailMessage.Subject = subject;

        using (var client = new SmtpClient(_smtpServer, _smtpPort))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_apiKey, _apiSecret);

            try
            {
                client.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Error: {0}", ex.StatusCode);
            }
        }
    }
}