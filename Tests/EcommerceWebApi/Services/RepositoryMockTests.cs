using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Repositories;
using EcommerceWebApi.Services;
using EcommerceWebApi.Utilities;
using Moq;
using Xunit;
using static EcommerceWebApi.Services.OrderService;

namespace EcommerceWebApi.Services.Tests
{
    public class OrderServiceEnumTests
    {
        [Fact]
        public void OrderResult_HasExpectedValues()
        {
            // Assert
            Assert.Equal(0, (int)OrderResult.Success);
            Assert.Equal(1, (int)OrderResult.Fail);
            Assert.Equal(2, (int)OrderResult.QuantityNotEnough);
            Assert.Equal(3, (int)OrderResult.ProductNotFound);
            Assert.Equal(4, (int)OrderResult.InvalidInput);
        }

        [Fact]
        public void OrderResult_Enum_HasFiveValues()
        {
            // Act
            var values = Enum.GetValues(typeof(OrderResult));

            // Assert
            Assert.Equal(5, values.Length);
        }
    }

    public class ProductServiceMockTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductRepository> _mockProductRepository;

        public ProductServiceMockTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepository.Object);
        }

        private Product CreateSampleProduct(int id = 1, string title = "Test Product")
        {
            return new Product
            {
                Id = id,
                Title = title,
                Price = 99.99f,
                Rating = 4.0f,
                Brand = "TestBrand",
                Category = "Electronics",
                Thumbnail = new Uri("https://example.com/img.jpg"),
                Quantity = 10
            };
        }

        [Fact]
        public void GetProductById_WithExistingId_ReturnsProduct()
        {
            // Arrange
            var product = CreateSampleProduct(1);
            _mockProductRepository.Setup(r => r.GetById(1)).Returns(product);

            // Act
            var result = _mockUnitOfWork.Object.Products.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Product", result.Title);
        }

        [Fact]
        public void GetProductById_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.GetById(999)).Returns((Product?)null);

            // Act
            var result = _mockUnitOfWork.Object.Products.GetById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InsertProductAsync_WithValidProduct_ReturnsTrue()
        {
            // Arrange
            var product = CreateSampleProduct();
            _mockProductRepository.Setup(r => r.InsertAsync(product)).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Products.InsertAsync(product);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateProductAsync_WithValidProduct_ReturnsTrue()
        {
            // Arrange
            var product = CreateSampleProduct();
            _mockProductRepository.Setup(r => r.UpdateAsync(product)).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Products.UpdateAsync(product);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Products.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }
    }

    public class OrderRepositoryMockTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOrderRepository> _mockOrderRepository;

        public OrderRepositoryMockTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockUnitOfWork.Setup(u => u.Orders).Returns(_mockOrderRepository.Object);
        }

        private Order CreateSampleOrder(string id = "order-1", string userId = "user-1")
        {
            return new Order
            {
                Id = id,
                UserId = userId,
                ProductList = new Dictionary<int, int> { { 1, 2 } },
                Created = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };
        }

        [Fact]
        public void GetOrderById_WithExistingId_ReturnsOrder()
        {
            // Arrange
            var order = CreateSampleOrder();
            _mockOrderRepository.Setup(r => r.GetById("order-1")).Returns(order);

            // Act
            var result = _mockUnitOfWork.Object.Orders.GetById("order-1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("order-1", result.Id);
        }

        [Fact]
        public void GetOrderById_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.GetById("non-existing")).Returns((Order?)null);

            // Act
            var result = _mockUnitOfWork.Object.Orders.GetById("non-existing");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InsertOrderAsync_WithValidOrder_ReturnsTrue()
        {
            // Arrange
            var order = CreateSampleOrder();
            _mockOrderRepository.Setup(r => r.InsertAsync(order)).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Orders.InsertAsync(order);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateOrderAsync_WithValidOrder_ReturnsTrue()
        {
            // Arrange
            var order = CreateSampleOrder();
            _mockOrderRepository.Setup(r => r.UpdateAsync(order)).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Orders.UpdateAsync(order);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.DeleteAsync("order-1")).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Orders.DeleteAsync("order-1");

            // Assert
            Assert.True(result);
        }
    }

    public class UnitOfWorkTransactionTests
    {
        [Fact]
        public void IUnitOfWork_StartTransaction_IsCalledCorrectly()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();

            // Act
            mockUow.Object.StartTransaction();

            // Assert
            mockUow.Verify(u => u.StartTransaction(), Times.Once);
        }

        [Fact]
        public void IUnitOfWork_CommitTransaction_IsCalledCorrectly()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();

            // Act
            mockUow.Object.CommitTransaction();

            // Assert
            mockUow.Verify(u => u.CommitTransaction(), Times.Once);
        }

        [Fact]
        public void IUnitOfWork_AbortTransaction_IsCalledCorrectly()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();

            // Act
            mockUow.Object.AbortTransaction();

            // Assert
            mockUow.Verify(u => u.AbortTransaction(), Times.Once);
        }

        [Fact]
        public void IUnitOfWork_IsTransactionInProgress_ReturnsMockedValue()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.IsTransactionInProgress()).Returns(true);

            // Act
            var result = mockUow.Object.IsTransactionInProgress();

            // Assert
            Assert.True(result);
        }
    }
}
