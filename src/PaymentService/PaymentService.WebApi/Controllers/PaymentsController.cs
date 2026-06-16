using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.WebApi.UseCases.Payments.GetPaymentStatus;
using PaymentService.WebApi.UseCases.Payments.UpdatePaymentStatus;
using System.Threading.Tasks;

namespace PaymentService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("get/{orderId}")]
        public async Task<IActionResult> GetStatus(long orderId)
        {
            var result = await _mediator.Send(new GetPaymentStatusQuery(orderId));
            if (result == null) return NotFound("Платёж для данного заказа не найден");
            return Ok(result);
        }

        [HttpPut("updateStatus/{orderId}/{statusId}")]
        public async Task<IActionResult> UpdateStatus(long orderId, int statusId)
        {
            var success = await _mediator.Send(new UpdatePaymentStatusCommand(orderId));
            if (!success) return NotFound("Не удалось найти платёж для обновления");
            return Ok(new { message = "Статус платежа успешно обновлен на 'Paid' и событие отправлено в Kafka" });
        }
    }
}
