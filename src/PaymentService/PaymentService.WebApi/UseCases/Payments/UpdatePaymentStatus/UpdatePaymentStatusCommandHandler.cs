using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.DataAccess.Postgres;
using PaymentService.WebApi.Integration;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentService.WebApi.UseCases.Payments.UpdatePaymentStatus
{
    public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, bool>
    {
        private readonly AppDbContext _context;
        private readonly KafkaProducerService _kafkaProducer;

        public UpdatePaymentStatusCommandHandler(AppDbContext context, KafkaProducerService kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<bool> Handle(UpdatePaymentStatusCommand request, CancellationToken ct)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, ct);

            if (payment == null) return false;

            payment.Status = "Paid";
            await _context.SaveChangesAsync(ct);

            try
            {
                var paymentEvent = new
                {
                    OrderId = payment.OrderId,
                    Status = "Paid"
                };

                var jsonMessage = JsonSerializer.Serialize(paymentEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                await _kafkaProducer.PublishEventAsync("payment-events", jsonMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PaymentService] Ошибка отправки события оплаты: {ex.Message}");
            }

            return true;
        }
    }
}
