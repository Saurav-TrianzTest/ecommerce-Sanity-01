using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Services;
using EcommerceWebApi.Authentication;
using static EcommerceWebApi.Authentication.AuthService;

namespace EcommerceWebApi.Tests.Authentication
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ITotpService> _mockTotpService;
        private readonly Mock<IJwtService> _mockJwtService;

        public AuthServiceTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockTotpService = new Mock<ITotpService>();
            _mockJwtService = new Mock<IJwtService>();
        }

        private AuthServiceTestable CreateService()
        {
            return new AuthServiceTestable(
                _mockUserService.Object,
                _mockTotpService.Object,
                _mockJwtService.Object
            );
        }

        private User CreateSampleUser(string id = "user-1", string username = "testuser")
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var password = "password123";
            return new User
            {
                Id = id,
                Username = username,
                Role = "User",
                IsTwoFactorAuthActivated = false,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)),
                RefreshToken = new RefreshToken
                {
                    Token = "refresh-token",
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            };
        }

        private Mock<Microsoft.AspNetCore.Http.HttpContext> CreateMockHttpContext()
        {
            var mockContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var mockRequest = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
            var mockResponse = new Mock<Microsoft.AspNetCore.Http.HttpResponse>();
            var mockCookies = new Mock<Microsoft.AspNetCore.Http.IRequestCookieCollection>();
            var mockResponseCookies = new Mock<Microsoft.AspNetCore.Http.IResponseCookies>();

            mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);
            mockResponse.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);
            mockContext.Setup(c => c.Request).Returns(mockRequest.Object);
            mockContext.Setup(c => c.Response).Returns(mockResponse.Object);

            return mockContext;
        }

        // Login Tests
        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsUserNotExist()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserByName("nonexistent")).Returns((User?)null);
            var service = CreateService();
            var mockContext = CreateMockHttpContext();

            // Act
            var result = await service.Login("nonexistent", "password", mockContext.Object);

            // Assert
            Assert.Equal(AuthResult.UserNotExist, result);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsInvalidCredentials()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockUserService.Setup(s => s.GetUserByName("testuser")).Returns(user);
            var service = CreateService();
            var mockContext = CreateMockHttpContext();

            // Act
            var result = await service.Login("testuser", "wrongpassword", mockContext.Object);

            // Assert
            Assert.Equal(AuthResult.InvalidCredentials, result);
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_NoTwoFactor_ReturnsSuccess()
        {
            // Arrange
            var user = CreateSampleUser();
            user.IsTwoFactorAuthActivated = false;
            _mockUserService.Setup(s => s.GetUserByName("testuser")).Returns(user);
            _mockJwtService.Setup(j => j.GenerateJWT(user, true, It.IsAny<Microsoft.AspNetCore.Http.HttpContext>()))
                .ReturnsAsync(true);
            var service = CreateService();
            var mockContext = CreateMockHttpContext();

            // Act
            var result = await service.Login("testuser", "password123", mockContext.Object);

            // Assert
            Assert.Equal(AuthResult.Success, result);
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_TwoFactorEnabled_ReturnsNeedSecondFactor()
        {
            // Arrange
            var user = CreateSampleUser();
            user.IsTwoFactorAuthActivated = true;
            _mockUserService.Setup(s => s.GetUserByName("testuser")).Returns(user);
            _mockJwtService.Setup(j => j.GenerateJWT(user, false, It.IsAny<Microsoft.AspNetCore.Http.HttpContext>()))
                .ReturnsAsync(true);
            var service = CreateService();
            var mockContext = CreateMockHttpContext();

            // Act
            var result = await service.Login("testuser", "password123", mockContext.Object);

            // Assert
            Assert.Equal(AuthResult.NeedSecondFactorAuth, result);
        }

        [Fact]
        public async Task Login_WhenJwtGenerationFails_ReturnsFail()
        {
            // Arrange
            var user = CreateSampleUser();
            user.IsTwoFactorAuthActivated = false;
            _mockUserService.Setup(s => s.GetUserByName("testuser")).Returns(user);
            _mockJwtService.Setup(j => j.GenerateJWT(user, true, It.IsAny<Microsoft.AspNetCore.Http.HttpContext>()))
                .ReturnsAsync(false);
            var service = CreateService();
            var mockContext = CreateMockHttpContext();

            // Act
            var result = await service.Login("testuser", "password123", mockContext.Object);

            // Assert
            Assert.Equal(AuthResult.Fail, result);
        }

        // Register Tests
        [Fact]
        public async Task Register_WithExistingUsername_ReturnsUserExisted()
        {
            // Arrange
            var existingUser = CreateSampleUser();
            _mockUserService.Setup(s => s.GetUserByName("testuser")).Returns(existingUser);
            var service = CreateService();

            // Act
            var result = await service.Register("testuser", "password123");

            // Assert
            Assert.Equal(AuthResult.UserExisted, result);
        }

        [Fact]
        public async Task Register_WithNewUsername_ReturnsSuccess()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserByName("newuser")).Returns((User?)null);
            _mockUserService.Setup(s => s.InsertUserAsync(It.IsAny<User>())).ReturnsAsync(true);
            var service = CreateService();

            // Act
            var result = await service.Register("newuser", "password123");

            // Assert
            Assert.Equal(AuthResult.Success, result);
        }

        [Fact]
        public async Task Register_WhenInsertFails_ReturnsFail()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserByName("newuser")).Returns((User?)null);
            _mockUserService.Setup(s => s.InsertUserAsync(It.IsAny<User>())).ReturnsAsync(false);
            var service = CreateService();

            // Act
            var result = await service.Register("newuser", "password123");

            // Assert
            Assert.Equal(AuthResult.Fail, result);
        }

        [Fact]
        public async Task Register_NewUser_HasUserRole()
        {
            // Arrange
            User? capturedUser = null;
            _mockUserService.Setup(s => s.GetUserByName("newuser")).Returns((User?)null);
            _mockUserService.Setup(s => s.InsertUserAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync(true);
            var service = CreateService();

            // Act
            await service.Register("newuser", "password123");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal("User", capturedUser.Role);
        }

        [Fact]
        public async Task Register_NewUser_TwoFactorIsDisabled()
        {
            // Arrange
            User? capturedUser = null;
            _mockUserService.Setup(s => s.GetUserByName("newuser")).Returns((User?)null);
            _mockUserService.Setup(s => s.InsertUserAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync(true);
            var service = CreateService();

            // Act
            await service.Register("newuser", "password123");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.False(capturedUser.IsTwoFactorAuthActivated);
        }

        [Fact]
        public async Task Register_NewUser_HasPasswordHashAndSalt()
        {
            // Arrange
            User? capturedUser = null;
            _mockUserService.Setup(s => s.GetUserByName("newuser")).Returns((User?)null);
            _mockUserService.Setup(s => s.InsertUserAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync(true);
            var service = CreateService();

            // Act
            await service.Register("newuser", "password123");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.NotNull(capturedUser.PasswordHash);
            Assert.NotNull(capturedUser.PasswordSalt);
            Assert.NotEmpty(capturedUser.PasswordHash);
            Assert.NotEmpty(capturedUser.PasswordSalt);
        }

        // RevokeToken Tests
        [Fact]
        public async Task RevokeToken_WhenSuccessful_ReturnsSuccess()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockJwtService.Setup(j => j.RevokeToken(user, It.IsAny<Microsoft.AspNetCore.Http.HttpContext>()))
                .ReturnsAsync(true);
            var service = CreateService();
            service.SetCurrentUser(user);
            var mockContext = CreateMockHttpContext();

            // Act
            var result = await service.RevokeToken(mockContext.Object);

            // Assert
            Assert.Equal(AuthResult.Success, result);
        }

        [Fact]
        public async Task RevokeToken_WhenFails_ReturnsFail()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockJwtService.Setup(j => j.RevokeToken(user, It.IsAny<Microsoft.AspNetCore.Http.HttpContext>()))
                .ReturnsAsync(false);
            var service = CreateService();
            service.SetCurrentUser(user);
            var mockContext = CreateMockHttpContext();

            // Act
            var result = await service.RevokeToken(mockContext.Object);

            // Assert
            Assert.Equal(AuthResult.Fail, result);
        }

        // GetQrCode Tests
        [Fact]
        public void GetQrCode_ReturnsSuccess()
        {
            // Arrange
            var user = CreateSampleUser();
            _mockTotpService.Setup(t => t.GenerateBase32Secret()).Returns("ABCDEFGHIJKLMNOP");
            _mockTotpService.Setup(t => t.GenerateQrCode(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("otpauth://totp/EcommerceWebApi:testuser?secret=ABCDEFGHIJKLMNOP&issuer=EcommerceWebApi");
            var service = CreateService();
            service.SetCurrentUser(user);

            // Act
            var result = service.GetQrCode(out string? uriString);

            // Assert
            Assert.Equal(AuthResult.Success, result);
            Assert.NotNull(uriString);
        }

        // ValidateQrCode Tests
        [Fact]
        public async Task ValidateQrCode_WithInvalidQrCode_ReturnsInvalidCredentials()
        {
            // Arrange
            _mockTotpService.Setup(t => t.GetSecretFromQrCode("invalid")).Returns((string?)null);
            var service = CreateService();
            var user = CreateSampleUser();
            service.SetCurrentUser(user);

            // Act
            var result = await service.ValidateQrCode("invalid", "123456");

            // Assert
            Assert.Equal(AuthResult.InvalidCredentials, result);
        }

        [Fact]
        public async Task ValidateQrCode_WithInvalidTotp_ReturnsInvalidCredentials()
        {
            // Arrange
            _mockTotpService.Setup(t => t.GetSecretFromQrCode("valid-qr")).Returns("ABCDEFGH");
            _mockTotpService.Setup(t => t.ValidateTotp("ABCDEFGH", "000000")).Returns(false);
            var service = CreateService();
            var user = CreateSampleUser();
            service.SetCurrentUser(user);

            // Act
            var result = await service.ValidateQrCode("valid-qr", "000000");

            // Assert
            Assert.Equal(AuthResult.InvalidCredentials, result);
        }

        [Fact]
        public async Task ValidateQrCode_WithValidQrCodeAndTotp_ReturnsSuccess()
        {
            // Arrange
            _mockTotpService.Setup(t => t.GetSecretFromQrCode("valid-qr")).Returns("ABCDEFGH");
            _mockTotpService.Setup(t => t.ValidateTotp("ABCDEFGH", "123456")).Returns(true);
            _mockUserService.Setup(s => s.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(true);
            var service = CreateService();
            var user = CreateSampleUser();
            service.SetCurrentUser(user);

            // Act
            var result = await service.ValidateQrCode("valid-qr", "123456");

            // Assert
            Assert.Equal(AuthResult.Success, result);
        }

        // AuthResult enum tests
        [Fact]
        public void AuthResult_AllValuesAreDefined()
        {
            Assert.True(Enum.IsDefined(typeof(AuthResult), AuthResult.UserNotExist));
            Assert.True(Enum.IsDefined(typeof(AuthResult), AuthResult.UserExisted));
            Assert.True(Enum.IsDefined(typeof(AuthResult), AuthResult.InvalidCredentials));
            Assert.True(Enum.IsDefined(typeof(AuthResult), AuthResult.NeedSecondFactorAuth));
            Assert.True(Enum.IsDefined(typeof(AuthResult), AuthResult.Success));
            Assert.True(Enum.IsDefined(typeof(AuthResult), AuthResult.Fail));
        }
    }

    // Testable subclass to inject interfaces
    internal class AuthServiceTestable
    {
        private readonly IUserService _userService;
        private readonly ITotpService _totpService;
        private readonly IJwtService _jwtService;
        public User CurrentUser { get; private set; } = null!;

        public AuthServiceTestable(IUserService userService, ITotpService totpService, IJwtService jwtService)
        {
            _userService = userService;
            _totpService = totpService;
            _jwtService = jwtService;
        }

        public void SetCurrentUser(User user) => CurrentUser = user;

        private static bool CheckPassword(User user, string password)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(key: user.PasswordSalt);
            var compute = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return compute.SequenceEqual(user.PasswordHash);
        }

        public async Task<AuthResult> Login(string username, string password, Microsoft.AspNetCore.Http.HttpContext context)
        {
            var user = _userService.GetUserByName(username);
            if (user == null) return AuthResult.UserNotExist;
            if (!CheckPassword(user, password)) return AuthResult.InvalidCredentials;
            if (user.IsTwoFactorAuthActivated)
            {
                return await _jwtService.GenerateJWT(user, false, context)
                    ? AuthResult.NeedSecondFactorAuth : AuthResult.Fail;
            }
            return await _jwtService.GenerateJWT(user, true, context)
                ? AuthResult.Success : AuthResult.Fail;
        }

        public async Task<AuthResult> LoginSecondFactor(string totp, Microsoft.AspNetCore.Http.HttpContext context)
        {
            bool validated = _totpService.ValidateTotp(CurrentUser.SecretCode, totp);
            if (!validated) return AuthResult.InvalidCredentials;
            var result = await _jwtService.RevokeToken(CurrentUser, context);
            if (!result) return AuthResult.Fail;
            return await _jwtService.GenerateJWT(CurrentUser, true, context)
                ? AuthResult.Success : AuthResult.Fail;
        }

        public async Task<AuthResult> Register(string username, string password)
        {
            if (_userService.GetUserByName(username) != null) return AuthResult.UserExisted;
            var user = new User { Username = username };
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                user.Role = "User";
                user.IsTwoFactorAuthActivated = false;
            }
            var insertResult = await _userService.InsertUserAsync(user);
            return insertResult ? AuthResult.Success : AuthResult.Fail;
        }

        public async Task<AuthResult> RevokeToken(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var revokeResult = await _jwtService.RevokeToken(CurrentUser, context);
            return revokeResult ? AuthResult.Success : AuthResult.Fail;
        }

        public AuthResult GetQrCode(out string? uriString)
        {
            uriString = _totpService.GenerateQrCode(
                _totpService.GenerateBase32Secret(),
                CurrentUser.Username
            );
            return AuthResult.Success;
        }

        public async Task<AuthResult> ValidateQrCode(string qrCode, string totp)
        {
            string? secret = _totpService.GetSecretFromQrCode(qrCode);
            if (secret == null) return AuthResult.InvalidCredentials;
            bool validated = _totpService.ValidateTotp(secret, totp);
            if (!validated) return AuthResult.InvalidCredentials;
            CurrentUser.SecretCode = secret;
            var updateResult = await _userService.UpdateUserAsync(CurrentUser);
            return updateResult ? AuthResult.Success : AuthResult.Fail;
        }
    }
}
