using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.DataAccess.Postgres;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentService.WebApi.UseCases.Payments.GetPaymentStatus
{
    public class GetPaymentQueryHandler : IRequestHandler<GetPaymentStatusQuery, string?>
    {
        private readonly AppDbContext _context;
        public GetPaymentQueryHandler(AppDbContext context) => _context = context;

        public async Task<string?> Handle(GetPaymentStatusQuery request, CancellationToken ct)
        {
            var payment = await _context.Payments
                .Where(p => p.OrderId == request.OrderId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync(ct);
            return payment?.Status;
        }
    }
}
