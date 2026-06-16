using MediatR;

namespace PaymentService.WebApi.UseCases.Payments.UpdatePaymentStatus
{
    public record UpdatePaymentStatusCommand(long OrderId) : IRequest<bool>;
}
