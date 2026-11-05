using EcommerceWebApi.Entities;
using Xunit;

namespace EcommerceWebApi.Entities.Tests
{
    public class UserTests
    {
        [Fact]
        public void User_DefaultConstructor_GeneratesGuidId()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.NotNull(user.Id);
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
                Username = "testuser",
                PasswordSalt = salt,
                PasswordHash = hash,
                IsTwoFactorAuthActivated = true,
                SecretCode = "SECRETCODE"
            };
            user.Role = "Admin";

            // Assert
            Assert.Equal("testuser", user.Username);
            Assert.Equal(salt, user.PasswordSalt);
            Assert.Equal(hash, user.PasswordHash);
            Assert.True(user.IsTwoFactorAuthActivated);
            Assert.Equal("SECRETCODE", user.SecretCode);
            Assert.Equal("Admin", user.Role);
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
        public void User_SecretCode_DefaultIsNull()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Null(user.SecretCode);
        }

        [Fact]
        public void RefreshToken_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var token = new RefreshToken();

            // Assert
            Assert.Equal(default(DateTime), token.Created);
            Assert.Equal(default(DateTime), token.Expires);
        }

        [Fact]
        public void RefreshToken_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var created = DateTime.UtcNow;
            var expires = DateTime.UtcNow.AddDays(7);
            var token = new RefreshToken
            {
                Token = "test-refresh-token",
                Created = created,
                Expires = expires
            };

            // Assert
            Assert.Equal("test-refresh-token", token.Token);
            Assert.Equal(created, token.Created);
            Assert.Equal(expires, token.Expires);
        }

        [Fact]
        public void User_SetRefreshToken_ReturnsCorrectToken()
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
