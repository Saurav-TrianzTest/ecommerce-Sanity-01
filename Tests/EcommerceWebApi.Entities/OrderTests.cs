using EcommerceWebApi.Entities;
using Xunit;

namespace EcommerceWebApi.Entities.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_DefaultConstructor_GeneratesGuidId()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            Assert.NotNull(order.Id);
            Assert.True(Guid.TryParse(order.Id, out _));
        }

        [Fact]
        public void Order_TwoInstances_HaveDifferentIds()
        {
            // Arrange & Act
            var order1 = new Order();
            var order2 = new Order();

            // Assert
            Assert.NotEqual(order1.Id, order2.Id);
        }

        [Fact]
        public void Order_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var productList = new Dictionary<int, int> { { 1, 2 }, { 3, 4 } };
            var created = DateTime.UtcNow;
            var updated = DateTime.UtcNow.AddHours(1);
            var order = new Order
            {
                UserId = "user-123",
                ProductList = productList,
                Created = created,
                Updated = updated,
                Status = OrderStatus.Pending
            };

            // Assert
            Assert.Equal("user-123", order.UserId);
            Assert.Equal(productList, order.ProductList);
            Assert.Equal(created, order.Created);
            Assert.Equal(updated, order.Updated);
            Assert.Equal(OrderStatus.Pending, order.Status);
        }

        [Fact]
        public void Order_StatusEnum_PendingValue()
        {
            // Assert
            Assert.Equal(0, (int)OrderStatus.Pending);
        }

        [Fact]
        public void Order_StatusEnum_SuccessedValue()
        {
            // Assert
            Assert.Equal(1, (int)OrderStatus.Successed);
        }

        [Fact]
        public void Order_StatusEnum_CanceledValue()
        {
            // Assert
            Assert.Equal(2, (int)OrderStatus.Canceled);
        }

        [Fact]
        public void Order_SetStatusToCanceled_ReturnsCorrectStatus()
        {
            // Arrange
            var order = new Order { Status = OrderStatus.Pending };

            // Act
            order.Status = OrderStatus.Canceled;

            // Assert
            Assert.Equal(OrderStatus.Canceled, order.Status);
        }

        [Fact]
        public void Order_SetStatusToSuccessed_ReturnsCorrectStatus()
        {
            // Arrange
            var order = new Order { Status = OrderStatus.Pending };

            // Act
            order.Status = OrderStatus.Successed;

            // Assert
            Assert.Equal(OrderStatus.Successed, order.Status);
        }

        [Fact]
        public void Order_ProductList_CanContainMultipleItems()
        {
            // Arrange
            var order = new Order
            {
                ProductList = new Dictionary<int, int>
                {
                    { 1, 5 },
                    { 2, 3 },
                    { 10, 1 }
                }
            };

            // Assert
            Assert.Equal(3, order.ProductList.Count);
            Assert.Equal(5, order.ProductList[1]);
            Assert.Equal(3, order.ProductList[2]);
            Assert.Equal(1, order.ProductList[10]);
        }
    }
}
