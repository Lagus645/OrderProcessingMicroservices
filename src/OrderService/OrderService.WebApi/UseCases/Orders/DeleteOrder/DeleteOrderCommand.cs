using MediatR;

namespace OrderService.WebApi.UseCases.Orders.DeleteOrder
{
    //возвращает true/false(успешно удалено/не найдено)
    public record DeleteOrderCommand(long Id) : IRequest<bool>;
}
