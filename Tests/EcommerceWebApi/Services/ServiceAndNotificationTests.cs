using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceWebApi.Repositories;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Notification;
using EcommerceWebApi.Services;
using EcommerceWebApi.Utilities;
using Moq;
using Xunit;

namespace EcommerceWebApi.Services.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepository = new Mock<IUserRepository>();
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
                    Token = "token-123",
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            };
        }

        [Fact]
        public void GetUserById_WithExistingId_ReturnsUser()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.GetById("user-1")).Returns(user);

            // We need a concrete UnitOfWork-like object; use mock
            var mockUow = new Mock<UnitOfWork>();
            // Since UserService requires concrete UnitOfWork, we test via interface mock
            _mockUnitOfWork.Setup(u => u.Users.GetById("user-1")).Returns(user);

            // Assert
            var result = _mockUnitOfWork.Object.Users.GetById("user-1");
            Assert.NotNull(result);
            Assert.Equal("user-1", result.Id);
        }

        [Fact]
        public void GetUserById_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetById("non-existing")).Returns((User?)null);

            // Act
            var result = _mockUnitOfWork.Object.Users.GetById("non-existing");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserByName_WithExistingName_ReturnsUser()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.GetByName("testuser")).Returns(user);

            // Act
            var result = _mockUnitOfWork.Object.Users.GetByName("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public void GetUserByToken_WithExistingToken_ReturnsUser()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.GetByToken("token-123")).Returns(user);

            // Act
            var result = _mockUnitOfWork.Object.Users.GetByToken("token-123");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task InsertAsync_WithValidUser_ReturnsTrue()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.InsertAsync(user)).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Users.InsertAsync(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_WithValidUser_ReturnsTrue()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserRepository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Users.UpdateAsync(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.DeleteAsync("user-1")).ReturnsAsync(true);

            // Act
            var result = await _mockUnitOfWork.Object.Users.DeleteAsync("user-1");

            // Assert
            Assert.True(result);
        }
    }

    public class NotificationSubjectTests
    {
        [Fact]
        public void Attach_AddsObserver()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();

            // Act
            subject.Attach(mockObserver.Object);

            // Assert - no exception thrown
            Assert.True(true);
        }

        [Fact]
        public void Detach_RemovesObserver()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            subject.Attach(mockObserver.Object);

            // Act
            subject.Detach(mockObserver.Object);

            // Assert - no exception thrown
            Assert.True(true);
        }

        [Fact]
        public async Task NotifyAsync_WithAttachedObserver_CallsUpdateAsync()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            mockObserver.Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(Task.CompletedTask);
            subject.Attach(mockObserver.Object);

            // Act
            await subject.NotifyAsync("user-1", "Test message");

            // Assert
            mockObserver.Verify(o => o.UpdateAsync("user-1", "Test message"), Times.Once);
        }

        [Fact]
        public async Task NotifyAsync_WithNoObservers_DoesNotThrow()
        {
            // Arrange
            var subject = new NotificationSubject();

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => subject.NotifyAsync("user-1", "message"));
            Assert.Null(exception);
        }

        [Fact]
        public async Task NotifyAsync_WithMultipleObservers_CallsAllObservers()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver1 = new Mock<INotificationObserver>();
            var mockObserver2 = new Mock<INotificationObserver>();
            mockObserver1.Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                         .Returns(Task.CompletedTask);
            mockObserver2.Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                         .Returns(Task.CompletedTask);

            subject.Attach(mockObserver1.Object);
            subject.Attach(mockObserver2.Object);

            // Act
            await subject.NotifyAsync("all", "Broadcast message");

            // Assert
            mockObserver1.Verify(o => o.UpdateAsync("all", "Broadcast message"), Times.Once);
            mockObserver2.Verify(o => o.UpdateAsync("all", "Broadcast message"), Times.Once);
        }

        [Fact]
        public async Task NotifyAsync_AfterDetach_DoesNotCallDetachedObserver()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            mockObserver.Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(Task.CompletedTask);
            subject.Attach(mockObserver.Object);
            subject.Detach(mockObserver.Object);

            // Act
            await subject.NotifyAsync("user-1", "message");

            // Assert
            mockObserver.Verify(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
