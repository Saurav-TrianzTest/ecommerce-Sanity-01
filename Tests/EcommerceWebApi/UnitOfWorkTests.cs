using System;
using Xunit;
using EcommerceWebApi;

namespace EcommerceWebApi.Tests
{
    public class UnitOfWorkTests
    {
        [Fact]
        public void UnitOfWork_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            var uow = new UnitOfWork();

            // Assert
            Assert.NotNull(uow);
        }

        [Fact]
        public void UnitOfWork_DefaultConstructor_InitializesRepositories()
        {
            // Arrange & Act
            var uow = new UnitOfWork();

            // Assert
            Assert.NotNull(uow.Users);
            Assert.NotNull(uow.Products);
            Assert.NotNull(uow.Orders);
        }

        [Fact]
        public void IsTransactionInProgress_Initially_ReturnsFalse()
        {
            // Arrange
            var uow = new UnitOfWork();

            // Act
            var result = uow.IsTransactionInProgress();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void StartTransaction_WhenNoTransactionInProgress_StartsSuccessfully()
        {
            // Arrange
            var uow = new UnitOfWork();

            // Act
            uow.StartTransaction();

            // Assert
            Assert.True(uow.IsTransactionInProgress());
        }

        [Fact]
        public void StartTransaction_WhenTransactionAlreadyInProgress_ThrowsInvalidOperationException()
        {
            // Arrange
            var uow = new UnitOfWork();
            uow.StartTransaction();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => uow.StartTransaction());
        }

        [Fact]
        public void AbortTransaction_WhenTransactionInProgress_AbortsSuccessfully()
        {
            // Arrange
            var uow = new UnitOfWork();
            uow.StartTransaction();

            // Act
            uow.AbortTransaction();

            // Assert
            Assert.False(uow.IsTransactionInProgress());
        }

        [Fact]
        public void AbortTransaction_WhenNoTransactionInProgress_ThrowsInvalidOperationException()
        {
            // Arrange
            var uow = new UnitOfWork();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => uow.AbortTransaction());
        }

        [Fact]
        public void CommitTransaction_WhenNoTransactionInProgress_ThrowsInvalidOperationException()
        {
            // Arrange
            var uow = new UnitOfWork();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => uow.CommitTransaction());
        }

        [Fact]
        public void CommitTransaction_WhenTransactionInProgress_CommitsSuccessfully()
        {
            // Arrange
            var uow = new UnitOfWork();
            uow.StartTransaction();

            // Act
            uow.CommitTransaction();

            // Assert
            Assert.False(uow.IsTransactionInProgress());
        }

        [Fact]
        public void CommitTransaction_ExecutesAddedActions()
        {
            // Arrange
            var uow = new UnitOfWork();
            uow.StartTransaction();
            bool actionExecuted = false;
            uow.AddToTransaction(() => actionExecuted = true);

            // Act
            uow.CommitTransaction();

            // Assert
            Assert.True(actionExecuted);
        }

        [Fact]
        public void AddToTransaction_WhenNoTransactionInProgress_ThrowsInvalidOperationException()
        {
            // Arrange
            var uow = new UnitOfWork();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => uow.AddToTransaction(() => { }));
        }

        [Fact]
        public void AddToTransaction_WhenTransactionInProgress_AddsAction()
        {
            // Arrange
            var uow = new UnitOfWork();
            uow.StartTransaction();

            // Act & Assert (no exception)
            var ex = Record.Exception(() => uow.AddToTransaction(() => { }));
            Assert.Null(ex);

            uow.AbortTransaction();
        }

        [Fact]
        public void Dispose_DoesNotThrow()
        {
            // Arrange
            var uow = new UnitOfWork();

            // Act & Assert
            var ex = Record.Exception(() => uow.Dispose());
            Assert.Null(ex);
        }

        [Fact]
        public void StartAbortCycle_CanBeRepeated()
        {
            // Arrange
            var uow = new UnitOfWork();

            // Act
            uow.StartTransaction();
            uow.AbortTransaction();
            uow.StartTransaction();
            uow.AbortTransaction();

            // Assert
            Assert.False(uow.IsTransactionInProgress());
        }
    }

    public class ChangeSetTests
    {
        [Fact]
        public void ChangeSet_DefaultConstructor_CreatesEmptyActionsList()
        {
            // Arrange & Act
            var changeSet = new ChangeSet();

            // Assert
            Assert.NotNull(changeSet.Actions);
            Assert.Empty(changeSet.Actions);
        }

        [Fact]
        public void ChangeSet_AddAction_IncreasesCount()
        {
            // Arrange
            var changeSet = new ChangeSet();

            // Act
            changeSet.Actions.Add(() => { });

            // Assert
            Assert.Single(changeSet.Actions);
        }

        [Fact]
        public void ChangeSet_AddMultipleActions_AllPresent()
        {
            // Arrange
            var changeSet = new ChangeSet();

            // Act
            changeSet.Actions.Add(() => { });
            changeSet.Actions.Add(() => { });
            changeSet.Actions.Add(() => { });

            // Assert
            Assert.Equal(3, changeSet.Actions.Count);
        }

        [Fact]
        public void ChangeSet_ExecuteActions_RunsAllActions()
        {
            // Arrange
            var changeSet = new ChangeSet();
            int counter = 0;
            changeSet.Actions.Add(() => counter++);
            changeSet.Actions.Add(() => counter++);

            // Act
            foreach (var action in changeSet.Actions)
            {
                action();
            }

            // Assert
            Assert.Equal(2, counter);
        }
    }

    public class AppSettingsTests
    {
        [Fact]
        public void AppSettings_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            var settings = new AppSettings();

            // Assert
            Assert.NotNull(settings);
        }

        [Fact]
        public void AppSettings_SetSecret_ReturnsCorrectValue()
        {
            // Arrange
            var settings = new AppSettings();

            // Act
            settings.Secret = "my-super-secret-key";

            // Assert
            Assert.Equal("my-super-secret-key", settings.Secret);
        }

        [Fact]
        public void AppSettings_SetEmptySecret_ReturnsEmptyString()
        {
            // Arrange
            var settings = new AppSettings();

            // Act
            settings.Secret = "";

            // Assert
            Assert.Equal("", settings.Secret);
        }
    }
}
