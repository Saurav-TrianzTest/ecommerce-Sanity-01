using System;
using System.Collections.Generic;
using Xunit;
using EcommerceWebApi.Notification;
using Moq;

namespace EcommerceWebApi.Tests.Notification
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

            // Act
            subject.Attach(mockObserver.Object);

            // Assert - no exception thrown
            Assert.NotNull(subject);
        }

        [Fact]
        public void Detach_RemovesObserver_Successfully()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            subject.Attach(mockObserver.Object);

            // Act
            subject.Detach(mockObserver.Object);

            // Assert - no exception thrown
            Assert.NotNull(subject);
        }

        [Fact]
        public async System.Threading.Tasks.Task NotifyAsync_WithObservers_CallsUpdateOnAll()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver1 = new Mock<INotificationObserver>();
            var mockObserver2 = new Mock<INotificationObserver>();

            mockObserver1.Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockObserver2.Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            subject.Attach(mockObserver1.Object);
            subject.Attach(mockObserver2.Object);

            // Act
            await subject.NotifyAsync("user-1", "Test message");

            // Assert
            mockObserver1.Verify(o => o.UpdateAsync("user-1", "Test message"), Times.Once);
            mockObserver2.Verify(o => o.UpdateAsync("user-1", "Test message"), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task NotifyAsync_WithNoObservers_DoesNotThrow()
        {
            // Arrange
            var subject = new NotificationSubject();

            // Act & Assert - no exception
            await subject.NotifyAsync("user-1", "Test message");
            Assert.NotNull(subject);
        }

        [Fact]
        public async System.Threading.Tasks.Task NotifyAsync_AfterDetach_DoesNotCallDetachedObserver()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            mockObserver.Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            subject.Attach(mockObserver.Object);
            subject.Detach(mockObserver.Object);

            // Act
            await subject.NotifyAsync("user-1", "Test message");

            // Assert
            mockObserver.Verify(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async System.Threading.Tasks.Task NotifyAsync_WithAllUserId_NotifiesAll()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver = new Mock<INotificationObserver>();
            mockObserver.Setup(o => o.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            subject.Attach(mockObserver.Object);

            // Act
            await subject.NotifyAsync("all", "Broadcast message");

            // Assert
            mockObserver.Verify(o => o.UpdateAsync("all", "Broadcast message"), Times.Once);
        }

        [Fact]
        public void Attach_MultipleObservers_AllAttached()
        {
            // Arrange
            var subject = new NotificationSubject();
            var mockObserver1 = new Mock<INotificationObserver>();
            var mockObserver2 = new Mock<INotificationObserver>();
            var mockObserver3 = new Mock<INotificationObserver>();

            // Act
            subject.Attach(mockObserver1.Object);
            subject.Attach(mockObserver2.Object);
            subject.Attach(mockObserver3.Object);

            // Assert - no exception
            Assert.NotNull(subject);
        }
    }

    public class NotificationObserverTests
    {
        [Fact]
        public void NotificationObserver_Constructor_CreatesInstance()
        {
            // Arrange
            var mockHubContext = new Mock<Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub>>();

            // Act
            var observer = new NotificationObserver(mockHubContext.Object);

            // Assert
            Assert.NotNull(observer);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateAsync_WithAllUserId_SendsToAll()
        {
            // Arrange
            var mockHubContext = new Mock<Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub>>();
            var mockClients = new Mock<Microsoft.AspNetCore.SignalR.IHubClients>();
            var mockClientProxy = new Mock<Microsoft.AspNetCore.SignalR.IClientProxy>();

            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClientProxy.Setup(p => p.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var observer = new NotificationObserver(mockHubContext.Object);

            // Act
            await observer.UpdateAsync("all", "Broadcast message");

            // Assert
            mockClients.Verify(c => c.All, Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateAsync_WithSpecificUserId_SendsToUser()
        {
            // Arrange
            var mockHubContext = new Mock<Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub>>();
            var mockClients = new Mock<Microsoft.AspNetCore.SignalR.IHubClients>();
            var mockClientProxy = new Mock<Microsoft.AspNetCore.SignalR.IClientProxy>();

            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.User(It.IsAny<string>())).Returns(mockClientProxy.Object);
            mockClientProxy.Setup(p => p.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var observer = new NotificationObserver(mockHubContext.Object);

            // Act
            await observer.UpdateAsync("user-123", "Personal message");

            // Assert
            mockClients.Verify(c => c.User("user-123"), Times.Once);
        }
    }
}
