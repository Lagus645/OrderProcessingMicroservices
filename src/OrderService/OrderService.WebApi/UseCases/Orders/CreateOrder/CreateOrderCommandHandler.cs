using MediatR;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.Integration;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.WebApi.UseCases.Orders.CreateOrder
{

    // Отвечает за сохранение данных в PostgreSQL и отправку события в Kafka.
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, long>
    {
        private readonly AppDbContext _context;
        private readonly KafkaProducerService _kafkaProducer;

        public CreateOrderCommandHandler(AppDbContext context, KafkaProducerService kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        // Основной метод обработки бизнес-логики создания заказа.
        public async Task<long> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                ProductId = request.ProductId,
                Amount = request.Amount,
                EmailClient = request.EmailClient,
                Price = request.Price,
                PhoneNumber = request.PhoneNumber,
                Status = "Unpaid", // ИЗМЕНЕНО: Изначально не оплачен
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            try
            {
                var orderEvent = new
                {
                    Id = order.Id,
                    Price = order.Price,
                    Amount = order.Amount
                };

                var jsonMessage = JsonSerializer.Serialize(orderEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                await _kafkaProducer.PublishEventAsync("order-events", jsonMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderService] Ошибка отправки события Kafka: {ex.Message}");
            }

            return order.Id;
        }
    }
}
