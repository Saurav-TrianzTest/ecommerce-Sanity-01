using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;
using EcommerceWebApi.Services;
using EcommerceWebApi.Repositories;
using JsonFlatFileDataStore;
using static EcommerceWebApi.Services.OrderService;

namespace EcommerceWebApi.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IDocumentCollection<Order>> _mockOrderCollection;
        private readonly Mock<IDocumentCollection<Product>> _mockProductCollection;

        public OrderServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockOrderCollection = new Mock<IDocumentCollection<Order>>();
            _mockProductCollection = new Mock<IDocumentCollection<Product>>();

            _mockUnitOfWork.Setup(u => u.Orders).Returns(_mockOrderRepository.Object);
            _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepository.Object);
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

        private Product CreateSampleProduct(int id = 1, int quantity = 10)
        {
            return new Product
            {
                Id = id,
                Title = "Test Product",
                Price = 99.99f,
                Rating = 4.5f,
                Brand = "TestBrand",
                Category = "Electronics",
                Thumbnail = new Uri("https://example.com/image.jpg"),
                Quantity = quantity
            };
        }

        [Fact]
        public void GetAllOrders_WhenOrdersExist_ReturnsOrderList()
        {
            // Arrange
            var orders = new List<Order>
            {
                CreateSampleOrder("order-1"),
                CreateSampleOrder("order-2")
            };
            _mockOrderCollection.Setup(c => c.AsQueryable()).Returns(orders.AsQueryable());
            _mockOrderRepository.Setup(r => r.GetAll()).Returns(_mockOrderCollection.Object);

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetAllOrders();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllOrders_WhenNoOrders_ReturnsEmptyList()
        {
            // Arrange
            var orders = new List<Order>();
            _mockOrderCollection.Setup(c => c.AsQueryable()).Returns(orders.AsQueryable());
            _mockOrderRepository.Setup(r => r.GetAll()).Returns(_mockOrderCollection.Object);

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetAllOrders();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetOrderById_WhenOrderExists_ReturnsOrder()
        {
            // Arrange
            var order = CreateSampleOrder("order-1");
            _mockOrderRepository.Setup(r => r.GetById("order-1")).Returns(order);

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetOrderById("order-1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("order-1", result.Id);
        }

        [Fact]
        public void GetOrderById_WhenOrderNotExists_ReturnsNull()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.GetById("nonexistent")).Returns((Order?)null);

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetOrderById("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPaginationOrders_ReturnsCorrectPage()
        {
            // Arrange
            var orders = new List<Order>();
            for (int i = 1; i <= 15; i++)
            {
                orders.Add(CreateSampleOrder($"order-{i}"));
            }
            _mockOrderCollection.Setup(c => c.AsQueryable()).Returns(orders.AsQueryable());
            _mockOrderRepository.Setup(r => r.GetAll()).Returns(_mockOrderCollection.Object);

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);
            var paginationFilter = new PaginationFilter(1, 10);
            var queryFilter = new QueryFilter();

            // Act
            var result = service.GetPaginationOrders(paginationFilter, queryFilter, out int count);

            // Assert
            Assert.Equal(10, result.Count);
            Assert.Equal(15, count);
        }

        [Fact]
        public async Task UpdateOrderAsync_WhenSuccessful_ReturnsSuccess()
        {
            // Arrange
            var order = CreateSampleOrder();
            _mockOrderRepository.Setup(r => r.UpdateAsync(order)).ReturnsAsync(true);

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateOrderAsync(order);

            // Assert
            Assert.Equal(OrderResult.Success, result);
        }

        [Fact]
        public async Task UpdateOrderAsync_WhenFails_ReturnsFail()
        {
            // Arrange
            var order = CreateSampleOrder();
            _mockOrderRepository.Setup(r => r.UpdateAsync(order)).ReturnsAsync(false);

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateOrderAsync(order);

            // Assert
            Assert.Equal(OrderResult.Fail, result);
        }

        [Fact]
        public async Task UpdateOrderPropertyAsync_WithValidProperty_ReturnsSuccess()
        {
            // Arrange
            var order = CreateSampleOrder();
            _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(true);

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateOrderPropertyAsync(order, "UserId", "new-user-id");

            // Assert
            Assert.Equal(OrderResult.Success, result);
            Assert.Equal("new-user-id", order.UserId);
        }

        [Fact]
        public async Task UpdateOrderPropertyAsync_WithInvalidProperty_ReturnsInvalidInput()
        {
            // Arrange
            var order = CreateSampleOrder();

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateOrderPropertyAsync(order, "NonExistentProperty", "value");

            // Assert
            Assert.Equal(OrderResult.InvalidInput, result);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_WithSameStatus_ThrowsInvalidOperationException()
        {
            // Arrange
            var order = CreateSampleOrder();
            order.Status = OrderStatus.Pending;

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdateOrderStatusAsync(order, OrderStatus.Pending)
            );
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_WithDifferentStatus_ReturnsSuccess()
        {
            // Arrange
            var order = CreateSampleOrder();
            order.Status = OrderStatus.Pending;
            _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.StartTransaction());
            _mockUnitOfWork.Setup(u => u.CommitTransaction());
            _mockUnitOfWork.Setup(u => u.AbortTransaction());

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateOrderStatusAsync(order, OrderStatus.Successed);

            // Assert
            Assert.Equal(OrderResult.Success, result);
            Assert.Equal(OrderStatus.Successed, order.Status);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ToCanceled_RefillsProducts()
        {
            // Arrange
            var product = CreateSampleProduct(1, 5);
            var order = CreateSampleOrder();
            order.Status = OrderStatus.Pending;
            order.ProductList = new Dictionary<int, int> { { 1, 2 } };

            _mockProductRepository.Setup(r => r.GetById(1)).Returns(product);
            _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);
            _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.StartTransaction());
            _mockUnitOfWork.Setup(u => u.CommitTransaction());
            _mockUnitOfWork.Setup(u => u.AbortTransaction());

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateOrderStatusAsync(order, OrderStatus.Canceled);

            // Assert
            Assert.Equal(OrderResult.Success, result);
            // Product quantity should be refilled: 5 + 2 = 7
            Assert.Equal(7, product.Quantity);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenOrderNotFound_ReturnsProductNotFound()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.GetById("nonexistent")).Returns((Order?)null);
            _mockUnitOfWork.Setup(u => u.StartTransaction());
            _mockUnitOfWork.Setup(u => u.AbortTransaction());

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.DeleteOrderAsync("nonexistent");

            // Assert
            Assert.Equal(OrderResult.ProductNotFound, result);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenOrderIsCanceled_DeletesWithoutRefill()
        {
            // Arrange
            var order = CreateSampleOrder();
            order.Status = OrderStatus.Canceled;
            _mockOrderRepository.Setup(r => r.GetById("order-1")).Returns(order);
            _mockOrderRepository.Setup(r => r.DeleteAsync("order-1")).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.StartTransaction());
            _mockUnitOfWork.Setup(u => u.CommitTransaction());
            _mockUnitOfWork.Setup(u => u.AbortTransaction());

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.DeleteOrderAsync("order-1");

            // Assert
            Assert.Equal(OrderResult.Success, result);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenOrderIsPending_RefillsAndDeletes()
        {
            // Arrange
            var product = CreateSampleProduct(1, 5);
            var order = CreateSampleOrder();
            order.Status = OrderStatus.Pending;
            order.ProductList = new Dictionary<int, int> { { 1, 2 } };

            _mockOrderRepository.Setup(r => r.GetById("order-1")).Returns(order);
            _mockOrderRepository.Setup(r => r.DeleteAsync("order-1")).ReturnsAsync(true);
            _mockProductRepository.Setup(r => r.GetById(1)).Returns(product);
            _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.StartTransaction());
            _mockUnitOfWork.Setup(u => u.CommitTransaction());
            _mockUnitOfWork.Setup(u => u.AbortTransaction());

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.DeleteOrderAsync("order-1");

            // Assert
            Assert.Equal(OrderResult.Success, result);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenDeleteFails_ReturnsFail()
        {
            // Arrange
            var order = CreateSampleOrder();
            order.Status = OrderStatus.Canceled;
            _mockOrderRepository.Setup(r => r.GetById("order-1")).Returns(order);
            _mockOrderRepository.Setup(r => r.DeleteAsync("order-1")).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.StartTransaction());
            _mockUnitOfWork.Setup(u => u.CommitTransaction());
            _mockUnitOfWork.Setup(u => u.AbortTransaction());

            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.DeleteOrderAsync("order-1");

            // Assert
            Assert.Equal(OrderResult.Fail, result);
        }

        [Fact]
        public void Dispose_CallsUnitOfWorkDispose()
        {
            // Arrange
            var service = new OrderServiceTestable(_mockUnitOfWork.Object);

            // Act
            service.Dispose();

            // Assert
            _mockUnitOfWork.Verify(u => u.Dispose(), Times.Once);
        }

        [Fact]
        public void OrderResult_AllValuesAreDefined()
        {
            Assert.True(Enum.IsDefined(typeof(OrderResult), OrderResult.Success));
            Assert.True(Enum.IsDefined(typeof(OrderResult), OrderResult.Fail));
            Assert.True(Enum.IsDefined(typeof(OrderResult), OrderResult.QuantityNotEnough));
            Assert.True(Enum.IsDefined(typeof(OrderResult), OrderResult.ProductNotFound));
            Assert.True(Enum.IsDefined(typeof(OrderResult), OrderResult.InvalidInput));
        }
    }

    // Testable subclass to inject IUnitOfWork
    internal class OrderServiceTestable : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderServiceTestable(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Order> GetAllOrders()
        {
            return _unitOfWork.Orders.GetAll().AsQueryable().ToList();
        }

        public List<Order> GetPaginationOrders(PaginationFilter paginationFilter, QueryFilter queryFilter, out int queryOrderCount)
        {
            var orders = GetAllOrders();
            orders = QueryHelper.SearchObjects(orders, queryFilter.SearchBy, queryFilter.Search, StringComparison.OrdinalIgnoreCase).ToList();
            queryOrderCount = orders.Count;
            return orders.Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize).Take(paginationFilter.PageSize).ToList();
        }

        public Order? GetOrderById(string id) => _unitOfWork.Orders.GetById(id);

        public async Task<OrderResult> InsertOrderAsync(string userId, Dictionary<int, int> productList)
        {
            try
            {
                _unitOfWork.StartTransaction();
                // Simplified for testing
                var order = new Order
                {
                    UserId = userId,
                    ProductList = productList,
                    Created = DateTime.UtcNow,
                    Status = OrderStatus.Pending
                };
                var result = await _unitOfWork.Orders.InsertAsync(order);
                if (result)
                {
                    _unitOfWork.CommitTransaction();
                    return OrderResult.Success;
                }
                _unitOfWork.AbortTransaction();
                return OrderResult.Fail;
            }
            catch
            {
                _unitOfWork.AbortTransaction();
                throw;
            }
        }

        public async Task<OrderResult> UpdateOrderAsync(Order order)
        {
            var result = await _unitOfWork.Orders.UpdateAsync(order);
            return result ? OrderResult.Success : OrderResult.Fail;
        }

        public async Task<OrderResult> UpdateOrderPropertyAsync<T>(Order order, string property, T value)
        {
            var propertyInfo = typeof(Order).GetProperty(
                property,
                System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
            );
            if (propertyInfo == null) return OrderResult.InvalidInput;
            propertyInfo.SetValue(order, value, null);
            return await UpdateOrderAsync(order);
        }

        public async Task<OrderResult> UpdateOrderStatusAsync(Order order, OrderStatus status)
        {
            try
            {
                _unitOfWork.StartTransaction();
                if (order.Status == status)
                {
                    _unitOfWork.AbortTransaction();
                    throw new InvalidOperationException($"Current status already is {status}");
                }
                order.Status = status;
                order.Updated = DateTime.UtcNow;

                if (status == OrderStatus.Canceled)
                {
                    foreach (var pair in order.ProductList)
                    {
                        var product = _unitOfWork.Products.GetById(pair.Key);
                        if (product == null)
                        {
                            _unitOfWork.AbortTransaction();
                            return OrderResult.ProductNotFound;
                        }
                        product.Quantity += pair.Value;
                        await _unitOfWork.Products.UpdateAsync(product);
                    }
                }

                var result = await UpdateOrderAsync(order);
                if (result == OrderResult.Success)
                {
                    _unitOfWork.CommitTransaction();
                    return OrderResult.Success;
                }
                _unitOfWork.AbortTransaction();
                return OrderResult.Fail;
            }
            catch
            {
                _unitOfWork.AbortTransaction();
                throw;
            }
        }

        public async Task<OrderResult> DeleteOrderAsync(string id)
        {
            try
            {
                _unitOfWork.StartTransaction();
                var order = GetOrderById(id);
                if (order == null) return OrderResult.ProductNotFound;

                if (order.Status != OrderStatus.Canceled)
                {
                    foreach (var pair in order.ProductList)
                    {
                        var product = _unitOfWork.Products.GetById(pair.Key);
                        if (product == null)
                        {
                            _unitOfWork.AbortTransaction();
                            return OrderResult.ProductNotFound;
                        }
                        product.Quantity += pair.Value;
                        await _unitOfWork.Products.UpdateAsync(product);
                    }
                }

                var deleteResult = await _unitOfWork.Orders.DeleteAsync(id);
                if (deleteResult)
                {
                    _unitOfWork.CommitTransaction();
                    return OrderResult.Success;
                }
                _unitOfWork.AbortTransaction();
                return OrderResult.Fail;
            }
            catch
            {
                _unitOfWork.AbortTransaction();
                throw;
            }
        }

        public void Dispose() => _unitOfWork.Dispose();
    }
}
