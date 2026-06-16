using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.WebApi.UseCases.Orders.GetOrder
{
    //обработчик запроса на чтение данных заказа из PostgreSQL
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Order?>
    {
        private readonly AppDbContext _context;

        public GetOrderByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        }
    }
}
