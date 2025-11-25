using Application.Interfaces.Messaging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging
{
    public class RabbitMqEventBus : IEventBus
    {
        private IConnection? _connection;
        private readonly IConfiguration _configuration;

        public RabbitMqEventBus(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private async Task<IConnection> GetConnectionAsync()
        {
            if (_connection is not null)
                return _connection;
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"],
                UserName = _configuration["RabbitMQ:User"],
                Password = _configuration["RabbitMQ:Pass"]
            };
            _connection = await factory.CreateConnectionAsync();
            return _connection;
        }
        public async Task PublishAsync<T>(T @event, string routingKey)
        {
            var connection = await GetConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: "cuidarmed.events",
                type: ExchangeType.Topic,
                durable: true
            );
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties();

            await channel.BasicPublishAsync<BasicProperties>(
                exchange: "cuidarmed.events",
                routingKey: routingKey,
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: body
            );
        }
    }
}
