using EcommerceWebApi.Notification;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace EcommerceWebApi.Notification.Tests
{
    public class NotificationSubjectTests
    {
        [Fact]
        public void NotificationSubject_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            var subject = new NotificationSubject();

            // Assert
            Assert.NotNull(subject);
        }

        [Fact]
        public void Attach_AddsObserver_Successfully()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();

            // Act & Assert (no exception thrown)
            subject.Attach(mockObserver.Object);
        }

        [Fact]
        public void Detach_RemovesObserver_Successfully()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            subject.Attach(mockObserver.Object);

            // Act & Assert (no exception thrown)
            subject.Detach(mockObserver.Object);
        }

        [Fact]
        public async Task NotifyAsync_WithAttachedObserver_CallsUpdateAsync()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            mockObserver
                .Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            subject.Attach(mockObserver.Object);

            // Act
            await subject.NotifyAsync("user-123", "Test message");

            // Assert
            mockObserver.Verify(o => o.UpdateAsync("user-123", "Test message"), Times.Once);
        }

        [Fact]
        public async Task NotifyAsync_WithMultipleObservers_CallsAllObservers()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver1 = new Mock<INotificationObserver>();
            var mockObserver2 = new Mock<INotificationObserver>();
            mockObserver1
                .Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            mockObserver2
                .Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            subject.Attach(mockObserver1.Object);
            subject.Attach(mockObserver2.Object);

            // Act
            await subject.NotifyAsync("user-123", "Test message");

            // Assert
            mockObserver1.Verify(o => o.UpdateAsync("user-123", "Test message"), Times.Once);
            mockObserver2.Verify(o => o.UpdateAsync("user-123", "Test message"), Times.Once);
        }

        [Fact]
        public async Task NotifyAsync_AfterDetach_DoesNotCallObserver()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            mockObserver
                .Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            subject.Attach(mockObserver.Object);
            subject.Detach(mockObserver.Object);

            // Act
            await subject.NotifyAsync("user-123", "Test message");

            // Assert
            mockObserver.Verify(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task NotifyAsync_WithNoObservers_CompletesSuccessfully()
        {
            // Arrange
            var subject = new NotificationSubject();

            // Act & Assert (no exception)
            await subject.NotifyAsync("user-123", "Test message");
        }

        [Fact]
        public async Task NotifyAsync_WithAllUserId_PassesCorrectUserId()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            mockObserver
                .Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            subject.Attach(mockObserver.Object);

            // Act
            await subject.NotifyAsync("all", "Broadcast message");

            // Assert
            mockObserver.Verify(o => o.UpdateAsync("all", "Broadcast message"), Times.Once);
        }
    }

    public class NotificationObserverTests
    {
        [Fact]
        public async Task UpdateAsync_WithAllUserId_SendsToAllClients()
        {
            // Arrange
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(c => c.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            var observer = new NotificationObserver(mockHubContext.Object);

            // Act
            await observer.UpdateAsync("all", "Broadcast message");

            // Assert
            mockClients.Verify(c => c.All, Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithSpecificUserId_SendsToSpecificUser()
        {
            // Arrange
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.User(It.IsAny<string>())).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(c => c.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            var observer = new NotificationObserver(mockHubContext.Object);

            // Act
            await observer.UpdateAsync("user-123", "Personal message");

            // Assert
            mockClients.Verify(c => c.User("user-123"), Times.Once);
        }

        [Fact]
        public void NotificationObserver_Constructor_CreatesInstance()
        {
            // Arrange
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();

            // Act
            var observer = new NotificationObserver(mockHubContext.Object);

            // Assert
            Assert.NotNull(observer);
        }
    }

    public class NotificationHubTests
    {
        [Fact]
        public void NotificationHub_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            var hub = new NotificationHub();

            // Assert
            Assert.NotNull(hub);
        }
    }
}
