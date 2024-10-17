using Domain.Entities;
using Domain.Interfaces.Messaging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Messaging.Producers
{
    public class MotoProducer : IProducer<Moto>
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MotoProducer(IConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
        }

        public void Publish(Moto moto)
        {
            _channel.QueueDeclare(queue: "moto_cadastro",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = JsonConvert.SerializeObject(moto);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                 routingKey: "moto_cadastro",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
