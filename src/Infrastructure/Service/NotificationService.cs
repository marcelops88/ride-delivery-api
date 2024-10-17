using Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public void Notify(string message)
        {
            // Aqui você pode implementar o envio de emails, SMS, ou qualquer outro tipo de notificação
            _logger.LogInformation($"Notificação: {message}");
        }
    }

}
