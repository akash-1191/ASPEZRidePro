using EZRide_Project.Model.Entities;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EZRide_Project.Services
{
    public class EmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MailMessage
            {
                From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            email.To.Add(toEmail);

            using var smtp = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(email);
        }

        //Send pdf receipt in the email 
        public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachmentBytes, string attachmentFileName)
        {
            var email = new MailMessage
            {
                From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            email.To.Add(toEmail);

            if (attachmentBytes != null && attachmentBytes.Length > 0)
            {
                var stream = new MemoryStream(attachmentBytes);
                var attachment = new Attachment(stream, attachmentFileName, "application/pdf");
                email.Attachments.Add(attachment);
            }

            using var smtp = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(email);

            // Dispose the stream after send
            foreach (var att in email.Attachments)
            {
                att.ContentStream.Dispose();
            }
        }

    }
}
