using MediatR;
using PaymentService.DataAccess.Postgres;
using PaymentService.DataAccess.Postgres.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentService.WebApi.UseCases.Payments.CreatePayment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, long>
    {
        private readonly AppDbContext _context;
        public CreatePaymentCommandHandler(AppDbContext context) => _context = context;

        public async Task<long> Handle(CreatePaymentCommand request, CancellationToken ct)
        {
            var payment = new Payment
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Status = "Unpaid", // ИЗМЕНЕНО: Изначально не оплачен
                CreatedAt = System.DateTime.UtcNow
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(ct);
            return payment.Id;
        }
    }
}
