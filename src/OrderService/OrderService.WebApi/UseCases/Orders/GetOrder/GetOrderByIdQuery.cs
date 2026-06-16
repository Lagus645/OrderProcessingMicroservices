using MediatR;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.WebApi.UseCases.Orders.GetOrder
{
    //возвращает объект order или null, если записи нет
    public record GetOrderByIdQuery(long Id) : IRequest<Order?>;
}
