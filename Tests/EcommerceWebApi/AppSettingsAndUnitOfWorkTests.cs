using System;
using System.Collections.Generic;
using EcommerceWebApi;
using Xunit;

namespace EcommerceWebApi.Tests
{
    public class AppSettingsTests
    {
        [Fact]
        public void AppSettings_SetSecret_ReturnsCorrectValue()
        {
            // Arrange & Act
            var settings = new AppSettings
            {
                Secret = "my-super-secret-key-for-jwt-token"
            };

            // Assert
            Assert.Equal("my-super-secret-key-for-jwt-token", settings.Secret);
        }

        [Fact]
        public void AppSettings_DefaultSecret_IsNull()
        {
            // Arrange & Act
            var settings = new AppSettings();

            // Assert - null! means it's null by default
            Assert.Null(settings.Secret);
        }

        [Fact]
        public void AppSettings_CanUpdateSecret()
        {
            // Arrange
            var settings = new AppSettings { Secret = "initial-secret" };

            // Act
            settings.Secret = "updated-secret";

            // Assert
            Assert.Equal("updated-secret", settings.Secret);
        }
    }

    public class ChangeSetTests
    {
        [Fact]
        public void ChangeSet_DefaultActions_IsEmptyList()
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
            Action action = () => { };

            // Act
            changeSet.Actions.Add(action);

            // Assert
            Assert.Single(changeSet.Actions);
        }

        [Fact]
        public void ChangeSet_AddMultipleActions_AllAreStored()
        {
            // Arrange
            var changeSet = new ChangeSet();
            int counter = 0;

            // Act
            changeSet.Actions.Add(() => counter++);
            changeSet.Actions.Add(() => counter++);
            changeSet.Actions.Add(() => counter++);

            // Execute all actions
            foreach (var action in changeSet.Actions)
            {
                action();
            }

            // Assert
            Assert.Equal(3, changeSet.Actions.Count);
            Assert.Equal(3, counter);
        }

        [Fact]
        public void ChangeSet_ActionsCanBeCleared()
        {
            // Arrange
            var changeSet = new ChangeSet();
            changeSet.Actions.Add(() => { });
            changeSet.Actions.Add(() => { });

            // Act
            changeSet.Actions.Clear();

            // Assert
            Assert.Empty(changeSet.Actions);
        }
    }

    public class UnitOfWorkTests
    {
        [Fact]
        public void UnitOfWork_Constructor_InitializesRepositories()
        {
            // Arrange & Act
            var unitOfWork = new UnitOfWork();

            // Assert
            Assert.NotNull(unitOfWork.Users);
            Assert.NotNull(unitOfWork.Products);
            Assert.NotNull(unitOfWork.Orders);
        }

        [Fact]
        public void UnitOfWork_IsTransactionInProgress_ReturnsFalseInitially()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act
            var result = unitOfWork.IsTransactionInProgress();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UnitOfWork_StartTransaction_SetsTransactionInProgress()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act
            unitOfWork.StartTransaction();

            // Assert
            Assert.True(unitOfWork.IsTransactionInProgress());
        }

        [Fact]
        public void UnitOfWork_StartTransaction_WhenAlreadyInProgress_ThrowsException()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();
            unitOfWork.StartTransaction();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => unitOfWork.StartTransaction());
        }

        [Fact]
        public void UnitOfWork_AbortTransaction_ClearsTransaction()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();
            unitOfWork.StartTransaction();

            // Act
            unitOfWork.AbortTransaction();

            // Assert
            Assert.False(unitOfWork.IsTransactionInProgress());
        }

        [Fact]
        public void UnitOfWork_AbortTransaction_WhenNoTransaction_ThrowsException()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => unitOfWork.AbortTransaction());
        }

        [Fact]
        public void UnitOfWork_CommitTransaction_WhenNoTransaction_ThrowsException()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => unitOfWork.CommitTransaction());
        }

        [Fact]
        public void UnitOfWork_CommitTransaction_AfterStartTransaction_ClearsTransaction()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();
            unitOfWork.StartTransaction();

            // Act
            unitOfWork.CommitTransaction();

            // Assert
            Assert.False(unitOfWork.IsTransactionInProgress());
        }

        [Fact]
        public void UnitOfWork_Dispose_DoesNotThrow()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act & Assert
            var exception = Record.Exception(() => unitOfWork.Dispose());
            Assert.Null(exception);
        }

        [Fact]
        public void UnitOfWork_StartAndAbortTransaction_CanStartAgain()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act
            unitOfWork.StartTransaction();
            unitOfWork.AbortTransaction();
            unitOfWork.StartTransaction(); // Should not throw

            // Assert
            Assert.True(unitOfWork.IsTransactionInProgress());
        }

        [Fact]
        public void UnitOfWork_MultipleDispose_DoesNotThrow()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act & Assert
            var exception = Record.Exception(() =>
            {
                unitOfWork.Dispose();
                unitOfWork.Dispose(); // Second dispose should be safe
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UnitOfWork_StartCommitAbort_SequenceWorks()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act
            unitOfWork.StartTransaction();
            Assert.True(unitOfWork.IsTransactionInProgress());
            unitOfWork.CommitTransaction();
            Assert.False(unitOfWork.IsTransactionInProgress());

            // Start again
            unitOfWork.StartTransaction();
            Assert.True(unitOfWork.IsTransactionInProgress());
            unitOfWork.AbortTransaction();
            Assert.False(unitOfWork.IsTransactionInProgress());
        }
    }
}
