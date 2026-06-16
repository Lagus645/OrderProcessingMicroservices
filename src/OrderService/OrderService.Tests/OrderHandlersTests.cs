using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OrderService.DataAccess.Postgres;
using OrderService.WebApi.UseCases.Orders.CreateOrder;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Tests
{
    [TestFixture]
    public class OrderHandlersTests
    {
        private AppDbContext _context = null!;
        private CreateOrderCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _handler = new CreateOrderCommandHandler(_context, null!);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateOrder_ShouldSaveOrderWithCorrectFieldsAndUnpaidStatus()
        {
            // Arrange — Передаем аргументы напрямую в конструктор согласно типам (long, int, string, decimal, string)
            var command = new CreateOrderCommand(
                1,                       // ProductId (long)
                1,                       // Amount (int)
                "test@example.com",      // EmailClient (string)
                24500m,                  // Price (decimal - обязательно с буквой m)
                "88005553535"            // PhoneNumber (string)
            );

            // Act
            var orderId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(orderId, Is.GreaterThan(0));

            var savedOrder = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            Assert.That(savedOrder, Is.Not.Null);
            Assert.That(savedOrder!.ProductId, Is.EqualTo(1));
            Assert.That(savedOrder.Price, Is.EqualTo(24500));
            Assert.That(savedOrder.EmailClient, Is.EqualTo("test@example.com"));
            Assert.That(savedOrder.PhoneNumber, Is.EqualTo("88005553535"));
            Assert.That(savedOrder.Status, Is.EqualTo("Unpaid"));
        }
    }
}
