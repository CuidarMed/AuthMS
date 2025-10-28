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
    public class ReservationPickedUpFormatter : INotificationFormatter
    {
        private static readonly JsonSerializerOptions _opts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public bool CanHandle(NotificationType type) =>
            type == NotificationType.VehiclePickedUp;

        public Task<string> FormatAsync(Notification n, User user)
        {
            var dto = JsonSerializer.Deserialize<ReservationPickedUpPayload>(n.Payload!, _opts)
                      ?? throw new InvalidOperationException("Payload inválido");

            var html = $@"
            <html>
              <body>
                <p>Hola {user.FirstName} {user.LastName},</p>
                <p>🚗 Tu reserva ha pasado a <b>En Curso</b> porque retiraste el vehículo.</p>
                <hr/>
                <p>📋 Detalles de la reserva:</p>   
                <p>🆔 <b>{dto.ReservationId}</b></p>
                <p>📍 Retiro: <b>{dto.PickupBranchName}</b></p>
                <p>🏁 Devolución: <b>{dto.DropOffBranchName}</b></p>
                <p>⏰ Hora real de retiro: <b>{dto.ActualPickupTime:dd/MM/yyyy HH:mm} hs</b></p>
                <p>🗓️ Hora prevista de devolucion: <b>{dto.EndTime:dd/MM/yyyy HH:mm} hs</b></p>
                <br/>
                <p>¡Buen viaje con Rentify!</p>
              </body>
            </html>";
            return Task.FromResult(html);
        }
    }
}
