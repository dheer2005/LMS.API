using LMS.API.DTOs;
using LMS.API.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace LMS.API.Services
{
    public class EmailSrvice: IEmailService
    {
        private readonly EmailSetting _setting;
        public EmailSrvice(IOptions<EmailSetting> mailSettings)
        {
            _setting = mailSettings.Value;
        }

        public async Task SendEmailAsync(EmailInfo emailInfo)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_setting.Email);
            email.To.Add(MailboxAddress.Parse(emailInfo.EmailTo));
            email.Subject = emailInfo.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = emailInfo.Body;  
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_setting.Host, _setting.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_setting.Email, _setting.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
