using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentService.WebApi.UseCases.Payments.CreatePayment;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentService.WebApi.Integration
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConsumerConfig _config;
        private const string Topic = "order-events";

        public KafkaConsumerService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:29092";
            _config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "payment-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
            return Task.CompletedTask;
        }

        private async Task StartConsumerLoop(CancellationToken stoppingToken)
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
            consumer.Subscribe(Topic);

            Console.WriteLine($"[Kafka Consumer] Подписался на топик {Topic}. Ожидаю сообщений...");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        if (consumeResult == null) continue;

                        Console.WriteLine($"[Kafka Consumer] Получено сообщение: {consumeResult.Message.Value}");

                        var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(consumeResult.Message.Value, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                        });

                        if (orderEvent != null)
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                            var command = new CreatePaymentCommand
                            {
                                OrderId = orderEvent.Id,
                                Amount = orderEvent.Price
                            };

                            var paymentId = await mediator.Send(command, stoppingToken);
                            Console.WriteLine($"[Kafka Consumer] Успешно создан платеж с ID {paymentId} для заказа {orderEvent.Id}");
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        Console.WriteLine($"[Kafka Consumer] Ошибка чтения сообщения: {ex.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Kafka Consumer] Ошибка обработки события: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }

    public class OrderCreatedEvent
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
    }
}
