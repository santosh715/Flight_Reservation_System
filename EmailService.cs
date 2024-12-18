using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
 
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
 
public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
 
    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }
 
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using (var client = new SmtpClient())
            {
                client.Host = _smtpSettings.Host;
                client.Port = _smtpSettings.Port;
                client.EnableSsl = _smtpSettings.EnableSSL; // Enables STARTTLS
                client.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
 
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.UserName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set to true if the email body contains HTML
                };
 
                mailMessage.To.Add(toEmail);
 
                await client.SendMailAsync(mailMessage);
            }
        }
        catch (SmtpException smtpEx)
        {
            // Log or rethrow for debugging
            throw new InvalidOperationException("SMTP Error: " + smtpEx.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new InvalidOperationException("Error sending email: " + ex.Message);
        }
    }
}
 
 