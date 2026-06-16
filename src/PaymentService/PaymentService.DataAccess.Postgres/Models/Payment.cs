using System;

namespace PaymentService.DataAccess.Postgres.Models
{
    public class Payment
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending"; // Возможные статусы: Pending, Paid, Failed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
