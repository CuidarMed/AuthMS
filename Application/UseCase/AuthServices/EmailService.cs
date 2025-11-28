using Application.Interfaces.IServices.IAuthServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Application.UseCase.AuthServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly bool _enableEmails;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _config = configuration;
            _logger = logger;

            // Configuración SMTP
            _smtpServer = _config["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.TryParse(_config["EmailSettings:SmtpPort"], out var port) ? port : 587;
            _senderEmail = _config["EmailSettings:SenderEmail"] ?? "cuidarmed.notificaciones@gmail.com";
            _senderPassword = _config["EmailSettings:SenderPassword"];
            _enableEmails = _config.GetValue("EmailSettings:EnableEmails", false);

            _logger.LogWarning("📧 EmailService inicializado. EnableEmails={Enable}", _enableEmails);

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
            _logger.LogWarning("📧 Preparando envío de email a {Email} (EnableEmails={Enable})", to, _enableEmails);

            // Modo desarrollo: solo loguea
            if (!_enableEmails)
            {
                _logger.LogWarning("📧 [DEV MODE] Email NO enviado. Solo se imprime en logs.");
                _logger.LogInformation("📧 DESTINATARIO: {Email}", to);
                _logger.LogInformation("📧 ASUNTO: {Subject}", subject);

                string preview = body.Length > 300 ? body.Substring(0, 300) + "..." : body;
                _logger.LogInformation("📧 CUERPO (300 chars): {Body}", preview);

                return;
            }

            try
            {
                _logger.LogWarning("📧 Enviando email REAL vía SMTP {Smtp}:{Port}", _smtpServer, _smtpPort);

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

                await smtp.SendMailAsync(mail);

                _logger.LogWarning("📧 EMAIL ENVIADO con éxito a {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR enviando email a {Email}", to);
                throw; // dejamos que el dispatcher marque FAILED
            }
        }
    }
}
