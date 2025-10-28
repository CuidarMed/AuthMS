using Application.Dtos.Request;
using Application.Interfaces.IQuery;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Interfaces.IServices.IAuthServices;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.UseCase.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IUserQuery _userQuery;
        private readonly IEmailService _emailService;

        public NotificationService(
            INotificationRepository repo,
            IUserQuery userQuery,
            IEmailService emailService)
        {
            _repo = repo;
            _userQuery = userQuery;
            _emailService = emailService;
        }
        

        public async Task EnqueueEvent(NotificationEventRequest request)
        {
            if (!Enum.TryParse<NotificationType>(request.EventType, out var type))
                throw new InvalidOperationException($"Tipo de evento '{request.EventType}' inválido.");

            // 1) Persisto notificación
            var notif = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.UserId,
                Type = type,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.Now,
                Payload = JsonSerializer.Serialize(request.Payload)
            };
            await _repo.Add(notif);
            
        }
    }
}
