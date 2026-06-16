using MediatR;
using OrderService.DataAccess.Postgres;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.WebApi.UseCases.Orders.DeleteOrder
{
    //обработчик команды удаления заказа
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteOrderCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object[] { request.Id }, cancellationToken);
            
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
