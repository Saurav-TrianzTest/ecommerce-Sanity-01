using EcommerceWebApi.Entities;
using EcommerceWebApi.Services;
using EcommerceWebApi.Utilities;
using Moq;
using Xunit;

namespace EcommerceWebApi.Services.Tests
{
    public class IUserServiceInterfaceTests
    {
        [Fact]
        public void IUserService_IsInterface()
        {
            // Assert
            Assert.True(typeof(IUserService).IsInterface);
        }

        [Fact]
        public void IUserService_HasGetAllUsersMethod()
        {
            // Assert
            var method = typeof(IUserService).GetMethod("GetAllUsers");
            Assert.NotNull(method);
        }

        [Fact]
        public void IUserService_HasGetPaginationUsersMethod()
        {
            // Assert
            var method = typeof(IUserService).GetMethod("GetPaginationUsers");
            Assert.NotNull(method);
        }

        [Fact]
        public void IUserService_HasGetUserByIdMethod()
        {
            // Assert
            var method = typeof(IUserService).GetMethod("GetUserById");
            Assert.NotNull(method);
        }

        [Fact]
        public void IUserService_HasGetUserByTokenMethod()
        {
            // Assert
            var method = typeof(IUserService).GetMethod("GetUserByToken");
            Assert.NotNull(method);
        }

        [Fact]
        public void IUserService_HasGetUserByNameMethod()
        {
            // Assert
            var method = typeof(IUserService).GetMethod("GetUserByName");
            Assert.NotNull(method);
        }

        [Fact]
        public void IUserService_HasInsertUserAsyncMethod()
        {
            // Assert
            var method = typeof(IUserService).GetMethod("InsertUserAsync");
            Assert.NotNull(method);
        }

        [Fact]
        public void IUserService_HasUpdateUserAsyncMethod()
        {
            // Assert
            var method = typeof(IUserService).GetMethod("UpdateUserAsync");
            Assert.NotNull(method);
        }

        [Fact]
        public void IUserService_HasDeleteUserAsyncMethod()
        {
            // Assert
            var method = typeof(IUserService).GetMethod("DeleteUserAsync");
            Assert.NotNull(method);
        }
    }

    public class IProductServiceInterfaceTests
    {
        [Fact]
        public void IProductService_IsInterface()
        {
            // Assert
            Assert.True(typeof(IProductService).IsInterface);
        }

        [Fact]
        public void IProductService_HasGetAllProductsMethod()
        {
            // Assert
            var method = typeof(IProductService).GetMethod("GetAllProducts");
            Assert.NotNull(method);
        }

        [Fact]
        public void IProductService_HasGetPaginationProductsMethod()
        {
            // Assert
            var method = typeof(IProductService).GetMethod("GetPaginationProducts");
            Assert.NotNull(method);
        }

        [Fact]
        public void IProductService_HasGetProductByIdMethod()
        {
            // Assert
            var method = typeof(IProductService).GetMethod("GetProductById");
            Assert.NotNull(method);
        }

        [Fact]
        public void IProductService_HasInsertProductAsyncMethod()
        {
            // Assert
            var method = typeof(IProductService).GetMethod("InsertProductAsync");
            Assert.NotNull(method);
        }

        [Fact]
        public void IProductService_HasUpdateProductAsyncMethod()
        {
            // Assert
            var method = typeof(IProductService).GetMethod("UpdateProductAsync");
            Assert.NotNull(method);
        }

        [Fact]
        public void IProductService_HasDeleteProductAsyncMethod()
        {
            // Assert
            var method = typeof(IProductService).GetMethod("DeleteProductAsync");
            Assert.NotNull(method);
        }
    }

    public class IOrderServiceInterfaceTests
    {
        [Fact]
        public void IOrderService_IsInterface()
        {
            // Assert
            Assert.True(typeof(IOrderService).IsInterface);
        }

        [Fact]
        public void IOrderService_HasGetAllOrdersMethod()
        {
            // Assert
            var method = typeof(IOrderService).GetMethod("GetAllOrders");
            Assert.NotNull(method);
        }

        [Fact]
        public void IOrderService_HasGetPaginationOrdersMethod()
        {
            // Assert
            var method = typeof(IOrderService).GetMethod("GetPaginationOrders");
            Assert.NotNull(method);
        }

        [Fact]
        public void IOrderService_HasGetOrderByIdMethod()
        {
            // Assert
            var method = typeof(IOrderService).GetMethod("GetOrderById");
            Assert.NotNull(method);
        }

        [Fact]
        public void IOrderService_HasInsertOrderAsyncMethod()
        {
            // Assert
            var method = typeof(IOrderService).GetMethod("InsertOrderAsync");
            Assert.NotNull(method);
        }

        [Fact]
        public void IOrderService_HasUpdateOrderAsyncMethod()
        {
            // Assert
            var method = typeof(IOrderService).GetMethod("UpdateOrderAsync");
            Assert.NotNull(method);
        }

        [Fact]
        public void IOrderService_HasUpdateOrderStatusAsyncMethod()
        {
            // Assert
            var method = typeof(IOrderService).GetMethod("UpdateOrderStatusAsync");
            Assert.NotNull(method);
        }

        [Fact]
        public void IOrderService_HasDeleteOrderAsyncMethod()
        {
            // Assert
            var method = typeof(IOrderService).GetMethod("DeleteOrderAsync");
            Assert.NotNull(method);
        }
    }

    public class OrderServiceOrderResultTests
    {
        [Fact]
        public void OrderResult_Enum_HasCorrectValues()
        {
            // Assert
            Assert.Equal(0, (int)OrderService.OrderResult.Success);
            Assert.Equal(1, (int)OrderService.OrderResult.Fail);
            Assert.Equal(2, (int)OrderService.OrderResult.QuantityNotEnough);
            Assert.Equal(3, (int)OrderService.OrderResult.ProductNotFound);
            Assert.Equal(4, (int)OrderService.OrderResult.InvalidInput);
        }

        [Fact]
        public void OrderResult_AllValuesAreDefined()
        {
            // Assert
            var values = Enum.GetValues(typeof(OrderService.OrderResult));
            Assert.Equal(5, values.Length);
        }

        [Fact]
        public void OrderResult_Success_IsDistinctFromFail()
        {
            // Assert
            Assert.NotEqual(OrderService.OrderResult.Success, OrderService.OrderResult.Fail);
        }

        [Fact]
        public void OrderResult_ProductNotFound_IsDistinctFromSuccess()
        {
            // Assert
            Assert.NotEqual(OrderService.OrderResult.ProductNotFound, OrderService.OrderResult.Success);
        }

        [Fact]
        public void OrderResult_InvalidInput_IsDistinctFromSuccess()
        {
            // Assert
            Assert.NotEqual(OrderService.OrderResult.InvalidInput, OrderService.OrderResult.Success);
        }
    }

    public class UserServiceMockTests
    {
        [Fact]
        public async Task IUserService_InsertUserAsync_ReturnsTrueOnSuccess()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var user = new User { Username = "testuser" };
            user.Role = "User";
            mockUserService.Setup(s => s.InsertUserAsync(user)).ReturnsAsync(true);

            // Act
            var result = await mockUserService.Object.InsertUserAsync(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IUserService_InsertUserAsync_ReturnsFalseOnFailure()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var user = new User { Username = "testuser" };
            mockUserService.Setup(s => s.InsertUserAsync(user)).ReturnsAsync(false);

            // Act
            var result = await mockUserService.Object.InsertUserAsync(user);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IUserService_GetUserById_ReturnsUser()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var user = new User { Username = "testuser" };
            mockUserService.Setup(s => s.GetUserById(It.IsAny<string>())).Returns(user);

            // Act
            var result = mockUserService.Object.GetUserById("some-id");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public void IUserService_GetUserById_ReturnsNullWhenNotFound()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetUserById(It.IsAny<string>())).Returns((User?)null);

            // Act
            var result = mockUserService.Object.GetUserById("non-existent-id");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void IUserService_GetAllUsers_ReturnsListOfUsers()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var users = new List<User>
            {
                new User { Username = "user1" },
                new User { Username = "user2" }
            };
            mockUserService.Setup(s => s.GetAllUsers()).Returns(users);

            // Act
            var result = mockUserService.Object.GetAllUsers();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void IUserService_GetPaginationUsers_ReturnsPagedUsers()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var filter = new PaginationFilter(1, 5);
            var users = new List<User> { new User { Username = "user1" } };
            mockUserService.Setup(s => s.GetPaginationUsers(filter)).Returns(users);

            // Act
            var result = mockUserService.Object.GetPaginationUsers(filter);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task IUserService_UpdateUserAsync_ReturnsTrueOnSuccess()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var user = new User { Username = "testuser" };
            mockUserService.Setup(s => s.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            var result = await mockUserService.Object.UpdateUserAsync(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IUserService_DeleteUserAsync_ReturnsTrueOnSuccess()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.DeleteUserAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await mockUserService.Object.DeleteUserAsync("user-id");

            // Assert
            Assert.True(result);
        }
    }

    public class ProductServiceMockTests
    {
        [Fact]
        public void IProductService_GetAllProducts_ReturnsListOfProducts()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();
            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Product 1", Brand = "Brand1", Price = 10.0f, Quantity = 5 },
                new Product { Id = 2, Title = "Product 2", Brand = "Brand2", Price = 20.0f, Quantity = 3 }
            };
            mockProductService.Setup(s => s.GetAllProducts()).Returns(products);

            // Act
            var result = mockProductService.Object.GetAllProducts();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void IProductService_GetProductById_ReturnsProduct()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();
            var product = new Product { Id = 1, Title = "Test Product", Brand = "Brand", Price = 10.0f, Quantity = 5 };
            mockProductService.Setup(s => s.GetProductById(1)).Returns(product);

            // Act
            var result = mockProductService.Object.GetProductById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void IProductService_GetProductById_ReturnsNullWhenNotFound()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(s => s.GetProductById(It.IsAny<int>())).Returns((Product?)null);

            // Act
            var result = mockProductService.Object.GetProductById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task IProductService_InsertProductAsync_ReturnsTrueOnSuccess()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();
            var product = new Product { Id = 1, Title = "New Product", Brand = "Brand", Price = 10.0f, Quantity = 5 };
            mockProductService.Setup(s => s.InsertProductAsync(product)).ReturnsAsync(true);

            // Act
            var result = await mockProductService.Object.InsertProductAsync(product);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IProductService_UpdateProductAsync_ReturnsTrueOnSuccess()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();
            var product = new Product { Id = 1, Title = "Updated Product", Brand = "Brand", Price = 15.0f, Quantity = 3 };
            mockProductService.Setup(s => s.UpdateProductAsync(product)).ReturnsAsync(true);

            // Act
            var result = await mockProductService.Object.UpdateProductAsync(product);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IProductService_DeleteProductAsync_ReturnsTrueOnSuccess()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(s => s.DeleteProductAsync(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await mockProductService.Object.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
        }
    }

    public class OrderServiceMockTests
    {
        [Fact]
        public void IOrderService_GetAllOrders_ReturnsListOfOrders()
        {
            // Arrange
            var mockOrderService = new Mock<IOrderService>();
            var orders = new List<Order>
            {
                new Order { UserId = "user1", Status = OrderStatus.Pending },
                new Order { UserId = "user2", Status = OrderStatus.Successed }
            };
            mockOrderService.Setup(s => s.GetAllOrders()).Returns(orders);

            // Act
            var result = mockOrderService.Object.GetAllOrders();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void IOrderService_GetOrderById_ReturnsOrder()
        {
            // Arrange
            var mockOrderService = new Mock<IOrderService>();
            var order = new Order { UserId = "user1", Status = OrderStatus.Pending };
            mockOrderService.Setup(s => s.GetOrderById(It.IsAny<string>())).Returns(order);

            // Act
            var result = mockOrderService.Object.GetOrderById("order-id");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void IOrderService_GetOrderById_ReturnsNullWhenNotFound()
        {
            // Arrange
            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(s => s.GetOrderById(It.IsAny<string>())).Returns((Order?)null);

            // Act
            var result = mockOrderService.Object.GetOrderById("non-existent-id");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task IOrderService_InsertOrderAsync_ReturnsSuccessOnSuccess()
        {
            // Arrange
            var mockOrderService = new Mock<IOrderService>();
            var productList = new Dictionary<int, int> { { 1, 2 } };
            mockOrderService
                .Setup(s => s.InsertOrderAsync(It.IsAny<string>(), It.IsAny<Dictionary<int, int>>()))
                .ReturnsAsync(OrderService.OrderResult.Success);

            // Act
            var result = await mockOrderService.Object.InsertOrderAsync("user-id", productList);

            // Assert
            Assert.Equal(OrderService.OrderResult.Success, result);
        }

        [Fact]
        public async Task IOrderService_UpdateOrderStatusAsync_ReturnsSuccessOnSuccess()
        {
            // Arrange
            var mockOrderService = new Mock<IOrderService>();
            var order = new Order { Status = OrderStatus.Pending };
            mockOrderService
                .Setup(s => s.UpdateOrderStatusAsync(order, OrderStatus.Successed))
                .ReturnsAsync(OrderService.OrderResult.Success);

            // Act
            var result = await mockOrderService.Object.UpdateOrderStatusAsync(order, OrderStatus.Successed);

            // Assert
            Assert.Equal(OrderService.OrderResult.Success, result);
        }

        [Fact]
        public async Task IOrderService_DeleteOrderAsync_ReturnsSuccessOnSuccess()
        {
            // Arrange
            var mockOrderService = new Mock<IOrderService>();
            mockOrderService
                .Setup(s => s.DeleteOrderAsync(It.IsAny<string>()))
                .ReturnsAsync(OrderService.OrderResult.Success);

            // Act
            var result = await mockOrderService.Object.DeleteOrderAsync("order-id");

            // Assert
            Assert.Equal(OrderService.OrderResult.Success, result);
        }
    }
}
