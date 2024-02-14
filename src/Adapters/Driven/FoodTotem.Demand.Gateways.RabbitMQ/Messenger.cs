using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System.Text;
using FoodTotem.Demand.Domain;
using RabbitMQ.Client.Events;

namespace FoodTotem.Demand.Gateways.RabbitMQ
{
    public class Messenger : IMessenger
    {
        private IConnection Connection { get; }
        private IModel Channel { get; }

        public Messenger(IConfiguration configuration)
        {
            var factory = new ConnectionFactory { 
                HostName = configuration.GetSection("RabbitMQ:HostName").Value,
                Port = int.Parse(configuration.GetSection("RabbitMQ:Port").Value!),
                UserName = configuration.GetSection("RabbitMQ:Username").Value,
                Password = configuration.GetSection("RabbitMQ:Password").Value
            };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
        }

        public void Send(string message, string queue)
        {
            Channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish(exchange: string.Empty,
                                 routingKey: queue,
                                 false,
                                 basicProperties: null,
                                 body: body);
        }

        public void Consume(string queue, Action<object> action)
        {
            Channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, ea) => action(ea);
            Channel.BasicConsume(queue: queue,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}