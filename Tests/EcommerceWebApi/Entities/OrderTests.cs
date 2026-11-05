using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EcommerceWebApi.Entities;
using Xunit;

namespace EcommerceWebApi.Entities.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_DefaultId_IsNotNullOrEmpty()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            Assert.NotNull(order.Id);
            Assert.NotEmpty(order.Id);
        }

        [Fact]
        public void Order_DefaultId_IsValidGuid()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
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
            var now = DateTime.UtcNow;
            var productList = new Dictionary<int, int> { { 1, 2 }, { 3, 4 } };
            var order = new Order
            {
                Id = "order-id",
                UserId = "user-id",
                ProductList = productList,
                Created = now,
                Updated = now,
                Status = OrderStatus.Pending
            };

            // Assert
            Assert.Equal("order-id", order.Id);
            Assert.Equal("user-id", order.UserId);
            Assert.Equal(productList, order.ProductList);
            Assert.Equal(now, order.Created);
            Assert.Equal(now, order.Updated);
            Assert.Equal(OrderStatus.Pending, order.Status);
        }

        [Fact]
        public void Order_Status_CanBeSetToPending()
        {
            // Arrange & Act
            var order = new Order { Status = OrderStatus.Pending };

            // Assert
            Assert.Equal(OrderStatus.Pending, order.Status);
        }

        [Fact]
        public void Order_Status_CanBeSetToSuccessed()
        {
            // Arrange & Act
            var order = new Order { Status = OrderStatus.Successed };

            // Assert
            Assert.Equal(OrderStatus.Successed, order.Status);
        }

        [Fact]
        public void Order_Status_CanBeSetToCanceled()
        {
            // Arrange & Act
            var order = new Order { Status = OrderStatus.Canceled };

            // Assert
            Assert.Equal(OrderStatus.Canceled, order.Status);
        }

        [Fact]
        public void OrderStatus_Enum_HasThreeValues()
        {
            // Arrange & Act
            var values = Enum.GetValues(typeof(OrderStatus));

            // Assert
            Assert.Equal(3, values.Length);
        }

        [Fact]
        public void Order_UserId_HasRequiredAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Order).GetProperty(nameof(Order.UserId));

            // Act
            var requiredAttr = propertyInfo?.GetCustomAttributes(typeof(RequiredAttribute), false);

            // Assert
            Assert.NotNull(requiredAttr);
            Assert.NotEmpty(requiredAttr);
        }

        [Fact]
        public void Order_ProductList_CanContainMultipleItems()
        {
            // Arrange
            var productList = new Dictionary<int, int>
            {
                { 1, 5 },
                { 2, 3 },
                { 10, 1 }
            };

            // Act
            var order = new Order { ProductList = productList };

            // Assert
            Assert.Equal(3, order.ProductList.Count);
            Assert.Equal(5, order.ProductList[1]);
            Assert.Equal(3, order.ProductList[2]);
            Assert.Equal(1, order.ProductList[10]);
        }
    }
}
