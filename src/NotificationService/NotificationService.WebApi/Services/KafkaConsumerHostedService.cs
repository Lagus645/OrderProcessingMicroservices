using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NotificationService.WebApi.Hubs;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.WebApi.Services
{
    public class PaymentSuccessEvent
    {
        public string Event { get; set; } = string.Empty;
        public long PaymentId { get; set; }
        public long OrderId { get; set; }
        public decimal Price { get; set; }
    }

    public class KafkaConsumerHostedService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ConsumerConfig _config;
        private const string Topic = "payment-events";

        public KafkaConsumerHostedService(IConfiguration configuration, IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
            
            var bootstrapServers = configuration["Kafka:BootstrapServers"];
            if (string.IsNullOrEmpty(bootstrapServers) || bootstrapServers == "localhost:9092")
            {
                bootstrapServers = "127.0.0.1:9092";
            }

            _config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "notification-group-v3",
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
            using var consumer = new ConsumerBuilder<Ignore, string>(_config)
                .SetErrorHandler((_, e) => Console.WriteLine($"[Kafka Internal Error] {e.Reason}"))
                .Build();

            consumer.Subscribe(Topic);
            Console.WriteLine($"[Kafka Consumer] Подключен к {_config.BootstrapServers}. Ожидание сообщений...");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    var rawMessage = consumeResult.Message.Value;
                    Console.WriteLine($"[Kafka Consumer] Перехвачено событие: {rawMessage}");

                    var paymentEvent = JsonSerializer.Deserialize<PaymentSuccessEvent>(rawMessage);

                    if (paymentEvent != null && paymentEvent.Event == "PaymentSuccess")
                    {
                        Console.WriteLine($"[SignalR] Отправка push-уведомления для OrderId: {paymentEvent.OrderId}");
                        
                        await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
                        {
                            Message = $"Ваш заказ №{paymentEvent.OrderId} успешно оплачен! Сумма: {paymentEvent.Price} руб.",
                            OrderId = paymentEvent.OrderId,
                            Status = "Paid"
                        }, cancellationToken);
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"[Kafka Consumer] Ошибка чтения: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Kafka Consumer] Ошибка обработки: {ex.Message}");
                }
            }

            consumer.Close();
        }
    }
}
