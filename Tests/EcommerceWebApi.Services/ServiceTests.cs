using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Services;
using EcommerceWebApi.Utilities;
using EcommerceWebApi.Repositories;

namespace EcommerceWebApi.Services.Tests
{
    // Fake implementations for testing without Moq
    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public JsonFlatFileDataStore.IDocumentCollection<User> GetAll()
            => throw new NotImplementedException("Use in-memory list instead");

        public User? GetById(string id) => _users.Find(u => u.Id == id);
        public User? GetByToken(string token) => _users.Find(u => u.RefreshToken?.Token == token);
        public User? GetByName(string name) => _users.Find(u => u.Username == name);

        public Task<bool> InsertAsync(User user)
        {
            _users.Add(user);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(User user)
        {
            var idx = _users.FindIndex(u => u.Id == user.Id);
            if (idx < 0) return Task.FromResult(false);
            _users[idx] = user;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(string id)
        {
            var user = _users.Find(u => u.Id == id);
            if (user == null) return Task.FromResult(false);
            _users.Remove(user);
            return Task.FromResult(true);
        }
    }

    public class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public JsonFlatFileDataStore.IDocumentCollection<Product> GetAll()
            => throw new NotImplementedException("Use in-memory list instead");

        public Product? GetById(int id) => _products.Find(p => p.Id == id);

        public Task<bool> InsertAsync(Product product)
        {
            _products.Add(product);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(Product product)
        {
            var idx = _products.FindIndex(p => p.Id == product.Id);
            if (idx < 0) return Task.FromResult(false);
            _products[idx] = product;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var product = _products.Find(p => p.Id == id);
            if (product == null) return Task.FromResult(false);
            _products.Remove(product);
            return Task.FromResult(true);
        }
    }

    public class FakeOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new();

        public JsonFlatFileDataStore.IDocumentCollection<Order> GetAll()
            => throw new NotImplementedException("Use in-memory list instead");

        public Order? GetById(string id) => _orders.Find(o => o.Id == id);

        public Task<bool> InsertAsync(Order order)
        {
            _orders.Add(order);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(Order order)
        {
            var idx = _orders.FindIndex(o => o.Id == order.Id);
            if (idx < 0) return Task.FromResult(false);
            _orders[idx] = order;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(string id)
        {
            var order = _orders.Find(o => o.Id == id);
            if (order == null) return Task.FromResult(false);
            _orders.Remove(order);
            return Task.FromResult(true);
        }
    }

    public class UserRepositoryTests
    {
        private User CreateTestUser(string id = "user-001", string username = "testuser")
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            return new User
            {
                Id = id,
                Username = username,
                Role = "User",
                IsTwoFactorAuthActivated = false,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("password")),
                RefreshToken = new RefreshToken
                {
                    Token = "token123",
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            };
        }

        [Fact]
        public void GetById_ExistingId_ReturnsUser()
        {
            // Arrange
            var repo = new FakeUserRepository();
            var user = CreateTestUser();
            repo.InsertAsync(user).Wait();

            // Act
            var result = repo.GetById("user-001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user-001", result.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNull()
        {
            // Arrange
            var repo = new FakeUserRepository();

            // Act
            var result = repo.GetById("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetByName_ExistingName_ReturnsUser()
        {
            // Arrange
            var repo = new FakeUserRepository();
            var user = CreateTestUser();
            repo.InsertAsync(user).Wait();

            // Act
            var result = repo.GetByName("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public void GetByName_NonExistingName_ReturnsNull()
        {
            // Arrange
            var repo = new FakeUserRepository();

            // Act
            var result = repo.GetByName("unknown");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetByToken_ExistingToken_ReturnsUser()
        {
            // Arrange
            var repo = new FakeUserRepository();
            var user = CreateTestUser();
            repo.InsertAsync(user).Wait();

            // Act
            var result = repo.GetByToken("token123");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetByToken_NonExistingToken_ReturnsNull()
        {
            // Arrange
            var repo = new FakeUserRepository();

            // Act
            var result = repo.GetByToken("badtoken");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InsertAsync_ValidUser_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeUserRepository();
            var user = CreateTestUser();

            // Act
            var result = await repo.InsertAsync(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeUserRepository();
            var user = CreateTestUser();
            await repo.InsertAsync(user);
            user.Username = "updateduser";

            // Act
            var result = await repo.UpdateAsync(user);

            // Assert
            Assert.True(result);
            Assert.Equal("updateduser", repo.GetById("user-001")?.Username);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingUser_ReturnsFalse()
        {
            // Arrange
            var repo = new FakeUserRepository();
            var user = CreateTestUser("nonexistent");

            // Act
            var result = await repo.UpdateAsync(user);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeUserRepository();
            var user = CreateTestUser();
            await repo.InsertAsync(user);

            // Act
            var result = await repo.DeleteAsync("user-001");

            // Assert
            Assert.True(result);
            Assert.Null(repo.GetById("user-001"));
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ReturnsFalse()
        {
            // Arrange
            var repo = new FakeUserRepository();

            // Act
            var result = await repo.DeleteAsync("nonexistent");

            // Assert
            Assert.False(result);
        }
    }

    public class ProductRepositoryTests
    {
        private Product CreateTestProduct(int id = 1, string title = "Test Product")
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
        public void GetById_ExistingId_ReturnsProduct()
        {
            // Arrange
            var repo = new FakeProductRepository();
            var product = CreateTestProduct();
            repo.InsertAsync(product).Wait();

            // Act
            var result = repo.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNull()
        {
            // Arrange
            var repo = new FakeProductRepository();

            // Act
            var result = repo.GetById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InsertAsync_ValidProduct_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeProductRepository();
            var product = CreateTestProduct();

            // Act
            var result = await repo.InsertAsync(product);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingProduct_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeProductRepository();
            var product = CreateTestProduct();
            await repo.InsertAsync(product);
            product.Title = "Updated Title";

            // Act
            var result = await repo.UpdateAsync(product);

            // Assert
            Assert.True(result);
            Assert.Equal("Updated Title", repo.GetById(1)?.Title);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingProduct_ReturnsFalse()
        {
            // Arrange
            var repo = new FakeProductRepository();
            var product = CreateTestProduct(999);

            // Act
            var result = await repo.UpdateAsync(product);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeProductRepository();
            var product = CreateTestProduct();
            await repo.InsertAsync(product);

            // Act
            var result = await repo.DeleteAsync(1);

            // Assert
            Assert.True(result);
            Assert.Null(repo.GetById(1));
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ReturnsFalse()
        {
            // Arrange
            var repo = new FakeProductRepository();

            // Act
            var result = await repo.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }

    public class OrderRepositoryTests
    {
        private Order CreateTestOrder(string id = "order-001")
        {
            return new Order
            {
                Id = id,
                UserId = "user-001",
                ProductList = new Dictionary<int, int> { { 1, 2 } },
                Created = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };
        }

        [Fact]
        public void GetById_ExistingId_ReturnsOrder()
        {
            // Arrange
            var repo = new FakeOrderRepository();
            var order = CreateTestOrder();
            repo.InsertAsync(order).Wait();

            // Act
            var result = repo.GetById("order-001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("order-001", result.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNull()
        {
            // Arrange
            var repo = new FakeOrderRepository();

            // Act
            var result = repo.GetById("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InsertAsync_ValidOrder_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeOrderRepository();
            var order = CreateTestOrder();

            // Act
            var result = await repo.InsertAsync(order);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingOrder_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeOrderRepository();
            var order = CreateTestOrder();
            await repo.InsertAsync(order);
            order.Status = OrderStatus.Successed;

            // Act
            var result = await repo.UpdateAsync(order);

            // Assert
            Assert.True(result);
            Assert.Equal(OrderStatus.Successed, repo.GetById("order-001")?.Status);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingOrder_ReturnsFalse()
        {
            // Arrange
            var repo = new FakeOrderRepository();
            var order = CreateTestOrder("nonexistent");

            // Act
            var result = await repo.UpdateAsync(order);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeOrderRepository();
            var order = CreateTestOrder();
            await repo.InsertAsync(order);

            // Act
            var result = await repo.DeleteAsync("order-001");

            // Assert
            Assert.True(result);
            Assert.Null(repo.GetById("order-001"));
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ReturnsFalse()
        {
            // Arrange
            var repo = new FakeOrderRepository();

            // Act
            var result = await repo.DeleteAsync("nonexistent");

            // Assert
            Assert.False(result);
        }
    }
}
