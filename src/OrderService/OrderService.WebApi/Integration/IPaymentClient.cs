using Refit;
using System.Threading.Tasks;

namespace OrderService.WebApi.Integration
{
    // Модель запроса строго по контракту ТЗ сервиса оплаты
    public class CreatePaymentRequest
    {
        public long OrderId { get; set; }
        public decimal Price { get; set; }
    }

    public interface IPaymentClient
    {
        // Описываем POST-запрос к микросервису оплаты
        [Post("/api/payments/create")]
        Task<long> CreatePaymentAsync([Body] CreatePaymentRequest request);
    }
}
