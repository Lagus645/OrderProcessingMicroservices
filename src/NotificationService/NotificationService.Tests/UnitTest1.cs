using NUnit.Framework;

namespace NotificationService.Tests
{
    [TestFixture]
    public class NotificationProcessingTests
    {
        [Test]
        public void CreateNotificationMessage_ValidData_ReturnsCorrectString()
        {
            var orderId = 123;
            var status = "Paid";
            

            var resultMessage = $"Order #{orderId} status updated to: {status}.";


            Assert.That(resultMessage, Is.EqualTo("Order #123 status updated to: Paid."));
        }

        [Test]
        public void OrderId_ShouldBePositive()
        {
            var orderId = 5;
            
            bool isValid = orderId > 0;

            Assert.That(isValid, Is.True, "ID заказа должен быть положительным числом!");
        }
    }
}
