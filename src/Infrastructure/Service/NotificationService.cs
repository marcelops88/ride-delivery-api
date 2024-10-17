using Domain.Interfaces.Messaging;
using Domain.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _webhookUrl;

        public NotificationService(ILogger<NotificationService> logger,
                                   HttpClient httpClient,
                                   IOptions<WebhookSettings> webhookSettings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _webhookUrl = webhookSettings.Value.Url;
        }

        public async Task NotifyAsync(string message)
        {
            var content = new StringContent(message, Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync(_webhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Notificação enviada com sucesso: {message}");
                }
                else
                {
                    _logger.LogWarning($"Falha ao enviar notificação. Status code: {response.StatusCode}, Mensagem: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação: {message}");
            }
        }
    }
}
