using MediatR;
using System.Text.Json.Serialization;

namespace PaymentService.WebApi.UseCases.Payments.CreatePayment
{
    public class CreatePaymentCommand : IRequest<long>
    {
        [JsonPropertyName("order_id")]
        public long OrderId { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
