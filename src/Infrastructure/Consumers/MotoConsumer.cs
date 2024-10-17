using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Messaging;
using Domain.Interfaces.Repositories;
using Domain.Models.Inputs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Infrastructure.Messaging.Consumers
{
    public class MotoConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;


        public MotoConsumer(IConnection connection, INotificationService notificationService, IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _notificationService = notificationService;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare(queue: "moto_cadastro",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var moto = JsonConvert.DeserializeObject<MotoInput>(message);
                await ProcessarMotoAsync(moto);
            };

            _channel.BasicConsume(queue: "moto_cadastro",
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessarMotoAsync(MotoInput moto)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var motoRepository = scope.ServiceProvider.GetRequiredService<IMotoRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var motoAdd = _mapper.Map<Moto>(moto);

                motoRepository.Add(motoAdd);

                if (moto.Ano == 2024)
                {
                    await _notificationService.NotifyAsync($"Moto do ano 2024 cadastrada: {moto.Identificador}");
                }
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
