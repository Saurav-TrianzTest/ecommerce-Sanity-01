using System;
using System.ComponentModel.DataAnnotations;
using EcommerceWebApi.Entities;
using Xunit;

namespace EcommerceWebApi.Entities.Tests
{
    public class UserTests
    {
        [Fact]
        public void User_DefaultId_IsNotNullOrEmpty()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.NotNull(user.Id);
            Assert.NotEmpty(user.Id);
        }

        [Fact]
        public void User_DefaultId_IsValidGuid()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.True(Guid.TryParse(user.Id, out _));
        }

        [Fact]
        public void User_TwoInstances_HaveDifferentIds()
        {
            // Arrange & Act
            var user1 = new User();
            var user2 = new User();

            // Assert
            Assert.NotEqual(user1.Id, user2.Id);
        }

        [Fact]
        public void User_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var salt = new byte[] { 1, 2, 3, 4 };
            var hash = new byte[] { 5, 6, 7, 8 };
            var user = new User
            {
                Id = "test-id",
                Username = "testuser",
                Role = "Admin",
                IsTwoFactorAuthActivated = true,
                PasswordSalt = salt,
                PasswordHash = hash,
                SecretCode = "secret123"
            };

            // Assert
            Assert.Equal("test-id", user.Id);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("Admin", user.Role);
            Assert.True(user.IsTwoFactorAuthActivated);
            Assert.Equal(salt, user.PasswordSalt);
            Assert.Equal(hash, user.PasswordHash);
            Assert.Equal("secret123", user.SecretCode);
        }

        [Fact]
        public void User_IsTwoFactorAuthActivated_DefaultIsFalse()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.False(user.IsTwoFactorAuthActivated);
        }

        [Fact]
        public void User_SecretCode_CanBeNull()
        {
            // Arrange & Act
            var user = new User { SecretCode = null };

            // Assert
            Assert.Null(user.SecretCode);
        }

        [Fact]
        public void User_Username_HasRequiredAttribute()
        {
            // Arrange
            var propertyInfo = typeof(User).GetProperty(nameof(User.Username));

            // Act
            var requiredAttr = propertyInfo?.GetCustomAttributes(typeof(RequiredAttribute), false);

            // Assert
            Assert.NotNull(requiredAttr);
            Assert.NotEmpty(requiredAttr);
        }

        [Fact]
        public void RefreshToken_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var expires = now.AddDays(7);
            var refreshToken = new RefreshToken
            {
                Token = "test-token",
                Created = now,
                Expires = expires
            };

            // Assert
            Assert.Equal("test-token", refreshToken.Token);
            Assert.Equal(now, refreshToken.Created);
            Assert.Equal(expires, refreshToken.Expires);
        }

        [Fact]
        public void RefreshToken_Token_HasRequiredAttribute()
        {
            // Arrange
            var propertyInfo = typeof(RefreshToken).GetProperty(nameof(RefreshToken.Token));

            // Act
            var requiredAttr = propertyInfo?.GetCustomAttributes(typeof(RequiredAttribute), false);

            // Assert
            Assert.NotNull(requiredAttr);
            Assert.NotEmpty(requiredAttr);
        }

        [Fact]
        public void User_RefreshToken_CanBeAssigned()
        {
            // Arrange
            var user = new User();
            var refreshToken = new RefreshToken
            {
                Token = "my-token",
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            // Act
            user.RefreshToken = refreshToken;

            // Assert
            Assert.Equal("my-token", user.RefreshToken.Token);
        }
    }
}
