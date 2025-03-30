using StackBook.Interfaces;
using System.Net.Mail;
using StackBook.Configurations;
using Microsoft.Extensions.Options;

namespace StackBook.Utils
{
    public class EMailUtils : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EMailUtils(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                client.EnableSsl = true;
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.From, _emailSettings.FromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(new MailAddress(email));
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}