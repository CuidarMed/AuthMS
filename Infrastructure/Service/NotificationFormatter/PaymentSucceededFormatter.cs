using Application.Dtos.Notification;
using Application.Interfaces.IServices;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Service.NotificationFormatter
{
    public class PaymentSucceededFormatter : INotificationFormatter
    {

        private static readonly JsonSerializerOptions _opts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public bool CanHandle(NotificationType type) =>
          type == NotificationType.PaymentSucceeded;

        public Task<string> FormatAsync(Notification n, User user)
        {
            var dto = JsonSerializer.Deserialize<PaymentSucceededPayload>(n.Payload!, _opts)
                      ?? throw new InvalidOperationException("Payload inválido");
            var html = $@"
          <html><body>
            <p>Hola {user.FirstName} {user.LastName},</p>
            <p>💰 Tu pago ha sido <b>procesado</b> con éxito.</p>
            <hr/>
            <p>📋 Detalles de pago:</p>
            <p>🆔 Reserva: <b>{dto.ReservationId}</b></p>
            <p>💵 Monto total: <b>${dto.TotalAmount:0.00}</b></p>
            <p>⚠️ Multa aplicada: <b>${dto.LateFee:0.00}</b></p>
            <p>🏦 Gateway: <b>{dto.PaymentGateway}</b></p>
            <p>🆔 Transacción: <b>{dto.TransactionId}</b></p>
            <br/>
            <p>¡Gracias por tu pago!</p>
          </body></html>";
            return Task.FromResult(html);
        }
    }
}
