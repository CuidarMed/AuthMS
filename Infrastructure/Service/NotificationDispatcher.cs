using Application.Dtos.Notification;
using Application.Interfaces.IQuery;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Interfaces.IServices.IAuthServices;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Service
{


    public class NotificationDispatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEnumerable<INotificationFormatter> _formatters;

        public NotificationDispatcher(
            IServiceScopeFactory scopeFactory,
            IEnumerable<INotificationFormatter> formatters)
        {
            _scopeFactory = scopeFactory;
            _formatters = formatters;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var userQuery = scope.ServiceProvider.GetRequiredService<IUserQuery>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var pendings = await repo.GetPending();
                foreach (var n in pendings)
                {
                    try
                    {
                        var user = await userQuery.GetUserById(n.UserId);
                        if (user == null) continue;

                        // elijo el primer formatter que canHandle (o el genérico al final)
                        var formatter = _formatters
                            .First(f => f.CanHandle(n.Type));

                        var body = await formatter.FormatAsync(n, user);

                        await emailService.SendCustomNotification(user.Email, body);

                        n.Status = NotificationStatus.Sent;
                        n.SentAt = DateTime.Now;
                    }
                    catch
                    {
                        n.Status = NotificationStatus.Failed;
                    }
                    await repo.Update(n);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        using var scope = _scopeFactory.CreateScope();
        //        var repo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        //        var userQuery = scope.ServiceProvider.GetRequiredService<IUserQuery>();
        //        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        //        var pendings = await repo.GetPending();
        //        var formatters = scope.ServiceProvider.GetServices<INotificationFormatter>();
        //        foreach (var n in pendings)
        //        {
        //            try
        //            {
        //                var user = await userQuery.GetUserById(n.UserId);
        //                if (user is null) throw new InvalidOperationException("Usuario no encontrado");

        //                // Selecciona el formatter apropiado (o uno genérico)
        //                var fmt = formatters.FirstOrDefault(f => f.CanHandle(n.Type))
        //                          ?? new DefaultFormatter(defaultMessages);

        //                var body = await fmt.FormatAsync(n, user);
        //                await emailService.SendCustomNotification(user.Email, body);

        //                n.Status = NotificationStatus.Sent;
        //                n.SentAt = DateTime.UtcNow;
        //            }
        //            catch
        //            {
        //                n.Status = NotificationStatus.Failed;
        //            }
        //            await repo.Update(n);
        //        }

        //        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        //    }
        //}
    }
}
