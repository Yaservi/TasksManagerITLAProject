using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AplicationLayer.Dtos.Email;
using AplicationLayer.Interfaces.Service;
using DomainLayer.Settings;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace InfrastructureLayer.Services
{
    public class EmailService(IOptions<EmailSetting> emailSettings) : IEmailService
    {
        private EmailSetting _emailSetting { get; } = emailSettings.Value;
        public async Task SendAsync(EmailRequestDto Request)
        {
            try
            {
                MimeMessage email = new();
                email.Sender = MailboxAddress.Parse(_emailSetting.EmailFrom);
                email.To.Add(MailboxAddress.Parse(Request.To));
                email.Subject = Request.Subject;
                BodyBuilder builder = new();
                builder.HtmlBody = Request.Body;
                email.Body = builder.ToMessageBody();
                using MailKit.Net.Smtp.SmtpClient smtp = new();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect(_emailSetting.SmtpHost, _emailSetting.SmtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailSetting.SmtpUser, _emailSetting.SmtpPass);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

            }
            catch (Exception)
            {

            }
        }
    }
}
