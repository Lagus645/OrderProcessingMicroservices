using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.WebApi.UseCases.Orders.CreateOrder;
using System.Threading.Tasks;

namespace OrderService.WebApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator) => _mediator = mediator;

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        // Добавляем метод для проверки статуса
        [HttpGet("status/{order_id:long}")]
        public async Task<IActionResult> GetStatus([FromRoute] long order_id)
        {
            // Здесь должна быть логика получения статуса, 
            // аналогичная той, что мы делали в PaymentService
            return Ok(new { order_id = order_id, status = "Pending" }); 
        }
    }
}
