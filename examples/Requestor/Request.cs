using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Requestor
{
    internal class Request
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            var queueDeklaredOk = await channel.QueueDeclareAsync(queue: "replyto", durable: false, exclusive: false, autoDelete: false,
    arguments: null);
            
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += (model, ea) =>
            {
                string? correlationId = ea.BasicProperties.CorrelationId;
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Got {response} with correlation id: {correlationId}");
                return Task.CompletedTask;
            };


            await channel.BasicConsumeAsync("replyto", true, consumer);
        }


    }
}
