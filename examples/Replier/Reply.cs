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

            await channel.QueueDeclareAsync("request-queue", exclusive: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                Console.WriteLine($"Received Request: {ea.BasicProperties.CorrelationId}");

                var replyMessage = $"This is your reply: {ea.BasicProperties.CorrelationId}";
                var replyProperties = new BasicProperties
                {
                    CorrelationId = ea.BasicProperties.CorrelationId
                };
                var body = Encoding.UTF8.GetBytes(replyMessage);

                if (string.IsNullOrEmpty(ea.BasicProperties.ReplyTo))
                {
                    Console.WriteLine("No reply-to property set. Cannot send reply.");
                    return;
                }

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: ea.BasicProperties.ReplyTo,
                    mandatory: true, 
                    basicProperties: replyProperties, 
                    body: body
                );
            };

            await channel.BasicConsumeAsync(queue: "request-queue", autoAck: true, consumer: consumer);

            Console.WriteLine(" Waiting for request");
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }

    }
}
