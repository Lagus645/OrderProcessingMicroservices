using FluentValidation;

namespace OrderService.WebApi.UseCases.Orders.CreateOrder
{
    //проверяет корректность данных перед вызовом Handler
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("ProductId должен быть больше 0");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Количество товара должно быть больше 0");
            RuleFor(x => x.EmailClient).NotEmpty().EmailAddress().WithMessage("Некорректный формат Email");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Цена должна быть больше 0");
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?\d{10,15}$").WithMessage("Некорректный формат телефона");
        }
    }
}
