namespace OrderService.DataAccess.Postgres.Models
{
    public class Order
    {
        public long Id { get; set; } // Первичный ключ
        public long ProductId { get; set; } // 
        public int Amount { get; set; } // 
        public string EmailClient { get; set; } = string.Empty; // 
        public decimal Price { get; set; } // 
        public string PhoneNumber { get; set; } = string.Empty; // 
        
        // Дополнительные поля для бизнес-логики (статус оплаты)
        public string Status { get; set; } = "Created"; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
