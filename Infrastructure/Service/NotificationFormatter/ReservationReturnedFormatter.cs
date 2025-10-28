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
    public class ReservationReturnedFormatter : INotificationFormatter
    {
        private static readonly JsonSerializerOptions _opts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public bool CanHandle(NotificationType type) =>
            type == NotificationType.VehicleReturned;

        public Task<string> FormatAsync(Notification n, User user)
        {
            var dto = JsonSerializer.Deserialize<ReservationReturnedPayload>(n.Payload!, _opts)
                      ?? throw new InvalidOperationException("Payload inválido");

            var html = $@"
            <html>
              <body>
                <p>Hola {user.FirstName} {user.LastName},</p>
                <p>✅ Confirmamos que devolviste tu vehículo. Ahora tu reserva está <b>pendiente de pago</b>.</p>
                <hr/>
                <p>📋 Detalles de la reserva:</p>   
                <p>🆔 <b>{dto.ReservationId}</b></p>
                <p>📍 Retiro: <b>{dto.PickupBranchName}</b></p>
                <p>🏁 Devolución: <b>{dto.DropOffBranchName}</b></p>
                <p>⏰ Hora real de retiro: <b>{dto.ActualPickupTime:dd/MM/yyyy HH:mm}</b></p>
                <p>⏰ Hora real de devolución: <b>{dto.ActualReturnTime:dd/MM/yyyy HH:mm}</b></p>
                <p style=""margin-top:1em;"">
                Para completar el proceso, realiza el pago en nuestra plataforma.
                </p>
                <br/>
                <p>¡Gracias por seguir confiando en Rentify!</p>
              </body>
            </html>";

            return Task.FromResult(html);
        }
    }
}
