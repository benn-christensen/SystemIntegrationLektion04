using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Replier
{
    internal class Reply
    {
        private const string replyToQueueName = "replyto";

        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await CreateReplytoQueue(channel);

            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = replyToQueueName
            };

            var messageBytes = Encoding.UTF8.GetBytes("Hello, World");
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "request_queue",
            mandatory: true, basicProperties: props, body: messageBytes);

            Console.WriteLine(" [x] Awaiting RPC requests");
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }

        private static async Task CreateReplytoQueue(IChannel channel)
        {
            await channel.QueueDeclareAsync(queue: replyToQueueName, durable: false, exclusive: false, autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var replyTo = ea.BasicProperties.ReplyTo;
                var correlationId = ea.BasicProperties.CorrelationId;
                Console.WriteLine($"Received message: {Encoding.UTF8.GetString(body)}");
                var replyMessage = $"Reply to message with CorrelationId: {correlationId}";
                var replyBody = Encoding.UTF8.GetBytes(replyMessage);
                var replyProperties = new BasicProperties
                {
                    CorrelationId = correlationId
                };

                var responseBytes = Encoding.UTF8.GetBytes(replyMessage);
                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: replyTo,
                    mandatory: true, basicProperties: replyProperties, body: responseBytes);
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync("replyto", false, consumer);
        }
    }
}
