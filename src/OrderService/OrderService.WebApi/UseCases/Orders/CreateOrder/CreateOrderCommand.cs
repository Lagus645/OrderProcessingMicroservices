using MediatR;
using System.Text.Json.Serialization;

namespace OrderService.WebApi.UseCases.Orders.CreateOrder
{
    public record CreateOrderCommand(
        [property: JsonPropertyName("product_id")] long ProductId,
        [property: JsonPropertyName("amount")] int Amount,
        [property: JsonPropertyName("email_client")] string EmailClient,
        [property: JsonPropertyName("price")] decimal Price,
        [property: JsonPropertyName("phone_number")] string PhoneNumber
    ) : IRequest<long>;
}
