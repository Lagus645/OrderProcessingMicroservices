using MediatR;

namespace PaymentService.WebApi.UseCases.Payments.GetPaymentStatus
{
    public record GetPaymentStatusQuery(long OrderId) : IRequest<string?>;
}
