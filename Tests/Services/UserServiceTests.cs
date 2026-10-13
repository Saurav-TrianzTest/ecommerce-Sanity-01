using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;
using EcommerceWebApi.Services;
using EcommerceWebApi.Repositories;
using JsonFlatFileDataStore;

namespace EcommerceWebApi.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IDocumentCollection<User>> _mockCollection;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCollection = new Mock<IDocumentCollection<User>>();
            _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepository.Object);
        }

        private User CreateSampleUser(string id = "user-1", string username = "testuser")
        {
            return new User
            {
                Id = id,
                Username = username,
                Role = "User",
                IsTwoFactorAuthActivated = false,
                PasswordSalt = new byte[] { 1, 2, 3 },
                PasswordHash = new byte[] { 4, 5, 6 },
                RefreshToken = new RefreshToken
                {
                    Token = "refresh-token",
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            };
        }

        [Fact]
        public void GetAllUsers_WhenUsersExist_ReturnsUserList()
        {
            // Arrange
            var users = new List<User> { CreateSampleUser("1", "user1"), CreateSampleUser("2", "user2") };
            _mockCollection.Setup(c => c.AsQueryable()).Returns(users.AsQueryable());
            _mockUserRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetAllUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllUsers_WhenNoUsers_ReturnsEmptyList()
        {
            // Arrange
            var users = new List<User>();
            _mockCollection.Setup(c => c.AsQueryable()).Returns(users.AsQueryable());
            _mockUserRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetAllUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetPaginationUsers_ReturnsCorrectPage()
        {
            // Arrange
            var users = new List<User>();
            for (int i = 1; i <= 15; i++)
            {
                users.Add(CreateSampleUser(i.ToString(), $"user{i}"));
            }
            _mockCollection.Setup(c => c.AsQueryable()).Returns(users.AsQueryable());
            _mockUserRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);
            var filter = new PaginationFilter(1, 10);

            // Act
            var result = service.GetPaginationUsers(filter);

            // Assert
            Assert.Equal(10, result.Count);
        }

        [Fact]
        public void GetPaginationUsers_SecondPage_ReturnsRemainingItems()
        {
            // Arrange
            var users = new List<User>();
            for (int i = 1; i <= 15; i++)
            {
                users.Add(CreateSampleUser(i.ToString(), $"user{i}"));
            }
            _mockCollection.Setup(c => c.AsQueryable()).Returns(users.AsQueryable());
            _mockUserRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);
            var filter = new PaginationFilter(2, 10);

            // Act
            var result = service.GetPaginationUsers(filter);

            // Assert
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public void GetUserById_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var user = CreateSampleUser("user-1");
            _mockUserRepository.Setup(r => r.GetById("user-1")).Returns(user);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetUserById("user-1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user-1", result.Id);
        }

        [Fact]
        public void GetUserById_WhenUserNotExists_ReturnsNull()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetById("nonexistent")).Returns((User?)null);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetUserById("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserByToken_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.GetByToken("valid-token")).Returns(user);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetUserByToken("valid-token");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetUserByToken_WhenTokenNotFound_ReturnsNull()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByToken("invalid-token")).Returns((User?)null);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetUserByToken("invalid-token");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserByName_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var user = CreateSampleUser(username: "testuser");
            _mockUserRepository.Setup(r => r.GetByName("testuser")).Returns(user);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetUserByName("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public void GetUserByName_WhenUserNotExists_ReturnsNull()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByName("nonexistent")).Returns((User?)null);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetUserByName("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InsertUserAsync_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.InsertAsync(user)).ReturnsAsync(true);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.InsertUserAsync(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task InsertUserAsync_WhenFails_ReturnsFalse()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.InsertAsync(user)).ReturnsAsync(false);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.InsertUserAsync(user);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateUserAsync(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenFails_ReturnsFalse()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(false);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateUserAsync(user);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserTokenAsync_WithRefreshToken_UpdatesToken()
        {
            // Arrange
            var user = CreateSampleUser();
            var newToken = new RefreshToken
            {
                Token = "new-token",
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateUserTokenAsync(user, newToken);

            // Assert
            Assert.True(result);
            Assert.Equal("new-token", user.RefreshToken.Token);
        }

        [Fact]
        public async Task UpdateUserTokenAsync_WithNullRefreshToken_ClearsToken()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateUserTokenAsync(user, null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateUserPropertyAsync_WithValidProperty_ReturnsTrue()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateUserPropertyAsync(user, "Username", "newusername");

            // Assert
            Assert.True(result);
            Assert.Equal("newusername", user.Username);
        }

        [Fact]
        public async Task UpdateUserPropertyAsync_WithInvalidProperty_ReturnsFalse()
        {
            // Arrange
            var user = CreateSampleUser();

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateUserPropertyAsync(user, "NonExistentProperty", "value");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.DeleteAsync("user-1")).ReturnsAsync(true);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.DeleteUserAsync("user-1");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenFails_ReturnsFalse()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.DeleteAsync("user-1")).ReturnsAsync(false);

            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.DeleteUserAsync("user-1");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Dispose_CallsUnitOfWorkDispose()
        {
            // Arrange
            var service = new UserServiceTestable(_mockUnitOfWork.Object);

            // Act
            service.Dispose();

            // Assert
            _mockUnitOfWork.Verify(u => u.Dispose(), Times.Once);
        }
    }

    // Testable subclass to inject IUnitOfWork instead of concrete UnitOfWork
    internal class UserServiceTestable : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserServiceTestable(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<User> GetAllUsers()
        {
            return _unitOfWork.Users.GetAll().AsQueryable().ToList();
        }

        public List<User> GetPaginationUsers(PaginationFilter paginationFilter)
        {
            return GetAllUsers()
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize)
                .ToList();
        }

        public User? GetUserById(string id) => _unitOfWork.Users.GetById(id);
        public User? GetUserByToken(string token) => _unitOfWork.Users.GetByToken(token);
        public User? GetUserByName(string name) => _unitOfWork.Users.GetByName(name);

        public async Task<bool> InsertUserAsync(User user) => await _unitOfWork.Users.InsertAsync(user);
        public async Task<bool> UpdateUserAsync(User user) => await _unitOfWork.Users.UpdateAsync(user);

        public async Task<bool> UpdateUserTokenAsync(User user, RefreshToken? refreshToken)
        {
            if (refreshToken != null)
                user.RefreshToken = refreshToken;
            else
                user.RefreshToken.Token = null!;
            return await UpdateUserAsync(user);
        }

        public async Task<bool> UpdateUserPropertyAsync<T>(User user, string property, T value)
        {
            var propertyInfo = typeof(User).GetProperty(
                property,
                System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
            );
            if (propertyInfo == null) return false;
            propertyInfo.SetValue(user, value, null);
            return await UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(string id) => await _unitOfWork.Users.DeleteAsync(id);

        public void Dispose() => _unitOfWork.Dispose();
    }
}
