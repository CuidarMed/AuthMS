using Application.Interfaces.IServices.IAuthServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthServices
{
    public class EmailService : IEmailService
    {

        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = "smtp.gmail.com";
            _smtpPort = 587;
            _senderEmail = "rentify2025@gmail.com";
            _senderPassword = configuration["EmailSettings:SenderPassword"];

            if (string.IsNullOrEmpty(_senderPassword))
            {
                throw new Exception("Falta 'EmailSettings:SenderPassword' en User Secrets o config.");
            }
        }


        public async Task SendPasswordResetEmail(string email, string resetCode)
        {
            await SendEmailAsync(email,
                "Restablecimiento de contraseña",
                $"Tu código de restablecimiento es: {resetCode}");
        }

        public async Task SendEmailVerification(string email, string verificationCode)
        {
            await SendEmailAsync(email,
                "Verificación de cuenta",
                $"Tu código de verificación es: {verificationCode}");
        }

         public async Task SendCustomNotification(string email, string message)
        {            
            await SendEmailAsync(
                to: email,
                subject: "Notificación",
                body: message,
                isHtml: true
            );
        }

        private async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            using var smtp = new SmtpClient(_smtpServer, _smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var mail = new MailMessage(_senderEmail, to, subject, body)
            {
                IsBodyHtml = isHtml    // <-- aquí permites HTML
            };
            try
            {
                await smtp.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                // Logging opcional aquí
                throw new Exception("Error al enviar el correo electrónico", ex);
            }
        }

        
    }
}
