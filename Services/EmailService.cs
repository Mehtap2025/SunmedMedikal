using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using MailKit.Security;

namespace SunmedMedikal.Services
{
    public class EmailService 
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                Console.WriteLine("UYARI: Müşterinin E-Posta adresi kayıtlı değil.");
                return; 
            }

            try
            {
                var smtpHost = _config["SmtpSettings:Host"];
                // Portu 465 yerine JSON'dan (587) çekiyoruz
                var smtpPort = int.Parse(_config["SmtpSettings:Port"] ?? "587");
                var smtpUser = _config["SmtpSettings:Username"];
                var smtpPass = _config["SmtpSettings:Password"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("SUNMED Medikal", smtpUser));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // DÜZELTİLEN KISIM: Port 587 ve StartTls kullanıyoruz!
                    await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    await client.AuthenticateAsync(smtpUser, smtpPass);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                } 
            } 
            catch (Exception ex)
            {
                Console.WriteLine("MAIL HATASI:");
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}