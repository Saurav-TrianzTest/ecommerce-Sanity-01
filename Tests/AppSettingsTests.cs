using System;
using Xunit;
using EcommerceWebApi;

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
        public void AppSettings_Secret_CanBeSetToEmptyString()
        {
            // Arrange
            var settings = new AppSettings();

            // Act
            settings.Secret = "";

            // Assert
            Assert.Equal("", settings.Secret);
        }

        [Fact]
        public void AppSettings_Secret_CanBeSetToLongString()
        {
            // Arrange
            var settings = new AppSettings();
            var longSecret = new string('x', 256);

            // Act
            settings.Secret = longSecret;

            // Assert
            Assert.Equal(longSecret, settings.Secret);
        }

        [Fact]
        public void AppSettings_TwoInstances_AreIndependent()
        {
            // Arrange
            var settings1 = new AppSettings { Secret = "secret1" };
            var settings2 = new AppSettings { Secret = "secret2" };

            // Assert
            Assert.NotEqual(settings1.Secret, settings2.Secret);
        }
    }
}
