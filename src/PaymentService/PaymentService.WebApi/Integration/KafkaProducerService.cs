using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace PaymentService.WebApi.Integration
{
    public class KafkaProducerService
    {
        private readonly ProducerConfig _config;

        public KafkaProducerService(IConfiguration configuration)
        {
            // Читаем адрес брокера из конфигурации (по умолчанию localhost:9092)
            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            _config = new ProducerConfig { BootstrapServers = bootstrapServers };
        }

        public async Task PublishEventAsync(string topic, string message)
        {
            using var producer = new ProducerBuilder<Null, string>(_config).Build();
            try
            {
                var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                Console.WriteLine($"[Kafka] Сообщение успешно отправлено в топик {result.Topic}, раздел {result.Partition}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Kafka] КРИТИЧЕСКАЯ ОШИБКА отправки в Kafka: {ex.Message}");
            }
        }
    }
}
