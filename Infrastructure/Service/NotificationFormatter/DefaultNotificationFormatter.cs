using Application.Interfaces.IServices;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.NotificationFormatter
{
    public class DefaultNotificationFormatter : INotificationFormatter
    {
        private readonly Dictionary<NotificationType, string> _templates
          = new()
        {
      { NotificationType.PaymentSucceeded,  "Hemos recibido tu pago con éxito." },
      { NotificationType.Reminder,          "Te recordamos que tienes una reserva próxima." },
      { NotificationType.ReservationEndingSoon, "¡Atención! Tu reserva está por vencer en breve. Extiéndela ahora para evitar cargos adicionales." },
      { NotificationType.Overdue,           "Has excedido el tiempo de tu reserva. Se aplicarán cargos adicionales." },
              //...
        };

        public bool CanHandle(NotificationType type) => true; // capturamos TODO lo demás

        public Task<string> FormatAsync(Notification n, User user)
        {
            if (string.IsNullOrWhiteSpace(n.Payload) || n.Payload == "null")
            {
                if (_templates.TryGetValue(n.Type, out var tpl))
                    return Task.FromResult(tpl);
                return Task.FromResult("Tienes una nueva notificación de Rentify.");
            }

            // si viene payload “custom”
            return Task.FromResult(n.Payload!);
        }
    }
}
