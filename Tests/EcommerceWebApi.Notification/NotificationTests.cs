using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using EcommerceWebApi.Notification;
using Microsoft.AspNetCore.SignalR;

namespace EcommerceWebApi.Notification.Tests
{
    // Manual fake observer for testing
    public class FakeNotificationObserver : INotificationObserver
    {
        public List<(string UserId, string Message)> ReceivedNotifications { get; } = new();

        public Task UpdateAsync(string userId, string message)
        {
            ReceivedNotifications.Add((userId, message));
            return Task.CompletedTask;
        }
    }

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
        public void Attach_Observer_DoesNotThrow()
        {
            // Arrange
            var subject = new NotificationSubject();
            var observer = new FakeNotificationObserver();

            // Act & Assert
            var ex = Record.Exception(() => subject.Attach(observer));
            Assert.Null(ex);
        }

        [Fact]
        public void Detach_AttachedObserver_DoesNotThrow()
        {
            // Arrange
            var subject = new NotificationSubject();
            var observer = new FakeNotificationObserver();
            subject.Attach(observer);

            // Act & Assert
            var ex = Record.Exception(() => subject.Detach(observer));
            Assert.Null(ex);
        }

        [Fact]
        public async Task NotifyAsync_WithObserver_CallsUpdateAsync()
        {
            // Arrange
            var subject = new NotificationSubject();
            var observer = new FakeNotificationObserver();
            subject.Attach(observer);

            // Act
            await subject.NotifyAsync("user1", "Hello");

            // Assert
            Assert.Single(observer.ReceivedNotifications);
            Assert.Equal("user1", observer.ReceivedNotifications[0].UserId);
            Assert.Equal("Hello", observer.ReceivedNotifications[0].Message);
        }

        [Fact]
        public async Task NotifyAsync_WithMultipleObservers_CallsAllUpdateAsync()
        {
            // Arrange
            var subject = new NotificationSubject();
            var observer1 = new FakeNotificationObserver();
            var observer2 = new FakeNotificationObserver();
            subject.Attach(observer1);
            subject.Attach(observer2);

            // Act
            await subject.NotifyAsync("user1", "Test message");

            // Assert
            Assert.Single(observer1.ReceivedNotifications);
            Assert.Single(observer2.ReceivedNotifications);
            Assert.Equal("user1", observer1.ReceivedNotifications[0].UserId);
            Assert.Equal("user1", observer2.ReceivedNotifications[0].UserId);
        }

        [Fact]
        public async Task NotifyAsync_AfterDetach_DoesNotCallObserver()
        {
            // Arrange
            var subject = new NotificationSubject();
            var observer = new FakeNotificationObserver();
            subject.Attach(observer);
            subject.Detach(observer);

            // Act
            await subject.NotifyAsync("user1", "Hello");

            // Assert
            Assert.Empty(observer.ReceivedNotifications);
        }

        [Fact]
        public async Task NotifyAsync_NoObservers_CompletesSuccessfully()
        {
            // Arrange
            var subject = new NotificationSubject();

            // Act & Assert
            var ex = await Record.ExceptionAsync(() => subject.NotifyAsync("user1", "Hello"));
            Assert.Null(ex);
        }

        [Fact]
        public async Task NotifyAsync_WithAllUserId_CallsUpdateAsync()
        {
            // Arrange
            var subject = new NotificationSubject();
            var observer = new FakeNotificationObserver();
            subject.Attach(observer);

            // Act
            await subject.NotifyAsync("all", "Broadcast message");

            // Assert
            Assert.Single(observer.ReceivedNotifications);
            Assert.Equal("all", observer.ReceivedNotifications[0].UserId);
            Assert.Equal("Broadcast message", observer.ReceivedNotifications[0].Message);
        }

        [Fact]
        public async Task NotifyAsync_MultipleMessages_AllDelivered()
        {
            // Arrange
            var subject = new NotificationSubject();
            var observer = new FakeNotificationObserver();
            subject.Attach(observer);

            // Act
            await subject.NotifyAsync("user1", "Message 1");
            await subject.NotifyAsync("user2", "Message 2");

            // Assert
            Assert.Equal(2, observer.ReceivedNotifications.Count);
        }

        [Fact]
        public async Task AttachDetachAttach_ObserverReceivesNotifications()
        {
            // Arrange
            var subject = new NotificationSubject();
            var observer = new FakeNotificationObserver();

            // Act
            subject.Attach(observer);
            await subject.NotifyAsync("user1", "First");
            subject.Detach(observer);
            await subject.NotifyAsync("user1", "Second");
            subject.Attach(observer);
            await subject.NotifyAsync("user1", "Third");

            // Assert
            Assert.Equal(2, observer.ReceivedNotifications.Count);
            Assert.Equal("First", observer.ReceivedNotifications[0].Message);
            Assert.Equal("Third", observer.ReceivedNotifications[1].Message);
        }
    }
}
