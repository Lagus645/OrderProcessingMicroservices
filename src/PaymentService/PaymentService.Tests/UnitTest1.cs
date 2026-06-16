using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PaymentService.DataAccess.Postgres;
using PaymentService.DataAccess.Postgres.Models;
using PaymentService.WebApi.UseCases.Payments.CreatePayment;
using PaymentService.WebApi.UseCases.Payments.GetPaymentStatus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentService.Tests
{
    [TestFixture]
    public class PaymentHandlersTests
    {
        private AppDbContext _context = null!;
        private CreatePaymentCommandHandler _createHandler = null!;
        private GetPaymentQueryHandler _getQueryHandler = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _createHandler = new CreatePaymentCommandHandler(_context);
            _getQueryHandler = new GetPaymentQueryHandler(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreatePayment_ShouldSavePaymentWithUnpaidStatus()
        {
            var command = new CreatePaymentCommand 
            { 
                OrderId = 55, 
                Amount = 15000 
            };

            var paymentId = await _createHandler.Handle(command, CancellationToken.None);

            Assert.That(paymentId, Is.GreaterThan(0));
            
            var savedPayment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
            Assert.That(savedPayment, Is.Not.Null);
            Assert.That(savedPayment!.Status, Is.EqualTo("Unpaid"));
        }

        [Test]
        public async Task GetPaymentStatus_ShouldReturnCorrectStatus_WhenPaymentExists()
        {
            var existingPayment = new Payment
            {
                OrderId = 99,
                Amount = 24500,
                Status = "Paid",
                CreatedAt = DateTime.UtcNow
            };
            _context.Payments.Add(existingPayment);
            await _context.SaveChangesAsync();

            var query = new GetPaymentStatusQuery(99);
            var status = await _getQueryHandler.Handle(query, CancellationToken.None);

            Assert.That(status, Is.EqualTo("Paid"));
        }
    }
}
