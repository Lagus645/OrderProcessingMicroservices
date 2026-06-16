using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using OrderService.DataAccess.Postgres;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.WebApi.Integration
{
    public class PaymentEventsConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConsumerConfig _config;
        private const string Topic = "payment-events";

        public PaymentEventsConsumerService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            _config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "order-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
            return Task.CompletedTask;
        }

        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
            consumer.Subscribe(Topic);
            Console.WriteLine($"[Kafka Consumer OrderService] Подписался на топик {Topic}. Ожидаю событий оплаты...");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    var message = consumeResult.Message.Value;
                    Console.WriteLine($"[Kafka Consumer OrderService] Получено событие оплаты: {message}");

                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var paymentEvent = JsonSerializer.Deserialize<PaymentEventModel>(message, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });

                    if (paymentEvent != null)
                    {
                        // Находим заказ в базе order_db и меняем его статус
                        var order = await context.Orders.FindAsync(new object[] { paymentEvent.OrderId }, cancellationToken);
                        if (order != null)
                        {
                            order.Status = paymentEvent.Status;
                            await context.SaveChangesAsync(cancellationToken);
                            Console.WriteLine($"[OrderService] Статус заказа {order.Id} успешно изменен на '{order.Status}' в БД!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Kafka Consumer OrderService] Ошибка при обработке события: {ex.Message}");
                    await Task.Delay(2000, cancellationToken);
                }
            }
        }

        private class PaymentEventModel
        {
            [JsonPropertyName("order_id")]
            public long OrderId { get; set; }
            [JsonPropertyName("status")]
            public string Status { get; set; }
        }
    }
}
