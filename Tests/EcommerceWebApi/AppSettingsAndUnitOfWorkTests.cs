using EcommerceWebApi;
using Xunit;

namespace EcommerceWebApi.Tests
{
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
            settings.Secret = "my-super-secret-key-for-jwt-token-generation";

            // Assert
            Assert.Equal("my-super-secret-key-for-jwt-token-generation", settings.Secret);
        }

        [Fact]
        public void AppSettings_SecretProperty_IsSettable()
        {
            // Arrange
            var settings = new AppSettings { Secret = "test-secret" };

            // Assert
            Assert.Equal("test-secret", settings.Secret);
        }
    }

    public class UnitOfWorkTests
    {
        [Fact]
        public void UnitOfWork_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            UnitOfWork? unitOfWork = null;
            try
            {
                unitOfWork = new UnitOfWork();
                // Assert
                Assert.NotNull(unitOfWork);
                Assert.NotNull(unitOfWork.Users);
                Assert.NotNull(unitOfWork.Products);
                Assert.NotNull(unitOfWork.Orders);
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        [Fact]
        public void UnitOfWork_IsTransactionInProgress_ReturnsFalseInitially()
        {
            // Arrange
            using var unitOfWork = new UnitOfWork();

            // Act
            var result = unitOfWork.IsTransactionInProgress();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UnitOfWork_StartTransaction_SetsTransactionInProgress()
        {
            // Arrange
            using var unitOfWork = new UnitOfWork();

            // Act
            unitOfWork.StartTransaction();

            // Assert
            Assert.True(unitOfWork.IsTransactionInProgress());

            // Cleanup
            unitOfWork.AbortTransaction();
        }

        [Fact]
        public void UnitOfWork_StartTransaction_WhenAlreadyInProgress_ThrowsException()
        {
            // Arrange
            using var unitOfWork = new UnitOfWork();
            unitOfWork.StartTransaction();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => unitOfWork.StartTransaction());

            // Cleanup
            unitOfWork.AbortTransaction();
        }

        [Fact]
        public void UnitOfWork_AbortTransaction_ClearsTransaction()
        {
            // Arrange
            using var unitOfWork = new UnitOfWork();
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
            using var unitOfWork = new UnitOfWork();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => unitOfWork.AbortTransaction());
        }

        [Fact]
        public void UnitOfWork_CommitTransaction_WhenNoTransaction_ThrowsException()
        {
            // Arrange
            using var unitOfWork = new UnitOfWork();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => unitOfWork.CommitTransaction());
        }

        [Fact]
        public void UnitOfWork_CommitTransaction_ExecutesActionsAndClearsTransaction()
        {
            // Arrange
            using var unitOfWork = new UnitOfWork();
            unitOfWork.StartTransaction();
            bool actionExecuted = false;
            unitOfWork.AddToTransaction(() => actionExecuted = true);

            // Act
            unitOfWork.CommitTransaction();

            // Assert
            Assert.True(actionExecuted);
            Assert.False(unitOfWork.IsTransactionInProgress());
        }

        [Fact]
        public void UnitOfWork_AddToTransaction_WhenNoTransaction_ThrowsException()
        {
            // Arrange
            using var unitOfWork = new UnitOfWork();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => unitOfWork.AddToTransaction(() => { }));
        }

        [Fact]
        public void UnitOfWork_Dispose_CanBeCalledMultipleTimes()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act & Assert (no exception)
            unitOfWork.Dispose();
            unitOfWork.Dispose();
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
            changeSet.Actions.Add(() => { });

            // Assert
            Assert.Equal(2, changeSet.Actions.Count);
        }

        [Fact]
        public void ChangeSet_Actions_AreExecutable()
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
}
