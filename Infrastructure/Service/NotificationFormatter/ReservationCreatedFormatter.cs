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
    //public class ReservationCreatedFormatter : INotificationFormatter
    //{
    //    private static readonly JsonSerializerOptions _opts = new()
    //    {
    //        PropertyNameCaseInsensitive = true
    //    };

    //    public bool CanHandle(NotificationType type) =>
    //        type == NotificationType.ReservationCreated;

    //    public Task<string> FormatAsync(Notification n, User user)
    //    {
    //        var dto = JsonSerializer.Deserialize<ReservationCreatedPayload>(n.Payload!, _opts)
    //                  ?? throw new InvalidOperationException("Payload inválido");

    //        return Task.FromResult(
    //            $"Hola {user.FirstName} {user.LastName}, tu reserva ha sido creada:\n" +
    //            $"- N° de reserva: {dto.ReservationId}\n" +
    //            $"- Recogida en: «{dto.PickupBranchName}» el día {dto.StartTime:dd/MM/yyyy HH:mm} hs\n" +
    //            $"- Devolución en: «{dto.DropOffBranchName}» el día {dto.EndTime:dd/MM/yyyy HH:mm} hs"
    //        );

    //    }
    //}

    public class ReservationCreatedFormatter : INotificationFormatter
    {
        private static readonly JsonSerializerOptions _opts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public bool CanHandle(NotificationType type) =>
            type == NotificationType.ReservationCreated;

        public Task<string> FormatAsync(Notification n, User user)
        {
            var dto = JsonSerializer.Deserialize<ReservationPayload>(n.Payload!, _opts)
                      ?? throw new InvalidOperationException("Payload inválido");

            var html = $@"
            <html>
              <body>
                <p>Hola {user.FirstName} {user.LastName},</p>
                <p>✅ Tu reserva ha sido <b>creada</b> correctamente.</p>
                <hr/>
                <p>📋 Detalles de la reserva:</p>                
                <p>🆔 <b>{dto.ReservationId}</b></p>
                <p>📍 Retiro: <b>{dto.PickupBranchName}</b></p>
                <p>🏁 Devolución: <b>{dto.DropOffBranchName}</b></p>
                <p>🗓️ Inicio:  <b>{dto.StartTime:dd/MM/yyyy HH:mm} hs</b></p>
                <p>🗓️ Fin:  <b>{dto.EndTime:dd/MM/yyyy HH:mm} hs</b></p>
                <br>

                <p>¡Gracias por elegir Rentify!</p>
              </body>
            </html>";

            return Task.FromResult(html);
        }
    }
}
