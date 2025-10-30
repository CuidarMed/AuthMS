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
        private readonly IConfiguration _config;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly bool _enableEmails;

        public EmailService(IConfiguration configuration)
        {
            _config = configuration;

            // Configuración SMTP (desde .env o appsettings)
            _smtpServer = _config["EmailSettings:smtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.TryParse(_config["EmailSettings:smtpPort"], out var port) ? port : 587;
            _senderEmail = _config["EmailSettings:SenderEmail"] ?? "rentify2025@gmail.com";
            _senderPassword = _config["EmailSettings:SenderPassword"];
            _enableEmails = _config.GetValue("EmailSettings:EnableEmails", false);

            if (_enableEmails && string.IsNullOrEmpty(_senderPassword))
            {
                throw new Exception("Falta 'EmailSettings:SenderPassword' en la configuración.");
            }
        }

        public async Task SendPasswordResetEmail(string email, string resetCode)
        {
            await SendEmailAsync(email, "Restablecimiento de contraseña",
                $"Tu código de restablecimiento es: {resetCode}");
        }

        public async Task SendEmailVerification(string email, string verificationCode)
        {
            await SendEmailAsync(email, "Verificación de cuenta",
                $"Tu código de verificación es: {verificationCode}");
        }

        public async Task SendCustomNotification(string email, string message)
        {
            await SendEmailAsync(email, "Notificación", message, isHtml: true);
        }

        private async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            // 🔹 Si el envío está desactivado (modo desarrollo), no se lanza error
            if (!_enableEmails)
            {
                Console.WriteLine($"[DEV MODE] Simulación de envío de email a {to} con asunto '{subject}'.");
                return;
            }

            using var smtp = new SmtpClient(_smtpServer, _smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var mail = new MailMessage(_senderEmail, to, subject, body)
            {
                IsBodyHtml = isHtml
            };

            try
            {
                await smtp.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                throw new Exception("Error al enviar el correo electrónico", ex);
            }
        }
    }
}

