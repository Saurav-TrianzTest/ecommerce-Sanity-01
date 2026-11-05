using EcommerceWebApi.Authentication;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using static EcommerceWebApi.Authentication.AuthService;

namespace EcommerceWebApi.Authentication.Tests
{
    public class AuthServiceTests
    {
        public AuthServiceTests()
        {
            // AuthService constructor takes concrete types (UserService, TotpService, JwtService)
            // We test the AuthResult enum and interface contracts via mock-based tests
        }

        [Fact]
        public void AuthResult_Enum_HasCorrectValues()
        {
            // Assert
            Assert.Equal(0, (int)AuthResult.UserNotExist);
            Assert.Equal(1, (int)AuthResult.UserExisted);
            Assert.Equal(2, (int)AuthResult.InvalidCredentials);
            Assert.Equal(3, (int)AuthResult.NeedSecondFactorAuth);
            Assert.Equal(4, (int)AuthResult.Success);
            Assert.Equal(5, (int)AuthResult.Fail);
        }

        [Fact]
        public void AuthResult_AllValuesAreDefined()
        {
            // Assert
            var values = Enum.GetValues(typeof(AuthResult));
            Assert.Equal(6, values.Length);
        }

        [Fact]
        public void AuthResult_UserNotExist_IsDistinctFromSuccess()
        {
            // Assert
            Assert.NotEqual(AuthResult.UserNotExist, AuthResult.Success);
        }

        [Fact]
        public void AuthResult_InvalidCredentials_IsDistinctFromSuccess()
        {
            // Assert
            Assert.NotEqual(AuthResult.InvalidCredentials, AuthResult.Success);
        }

        [Fact]
        public void AuthResult_Fail_IsDistinctFromSuccess()
        {
            // Assert
            Assert.NotEqual(AuthResult.Fail, AuthResult.Success);
        }
    }

    public class IJwtServiceInterfaceTests
    {
        [Fact]
        public void IJwtService_IsInterface()
        {
            // Assert
            Assert.True(typeof(IJwtService).IsInterface);
        }

        [Fact]
        public void IJwtService_HasGenerateJWTMethod()
        {
            // Assert
            var method = typeof(IJwtService).GetMethod("GenerateJWT");
            Assert.NotNull(method);
        }

        [Fact]
        public void IJwtService_HasSetJWTMethod()
        {
            // Assert
            var method = typeof(IJwtService).GetMethod("SetJWT");
            Assert.NotNull(method);
        }

        [Fact]
        public void IJwtService_HasSetRefreshTokenMethod()
        {
            // Assert
            var method = typeof(IJwtService).GetMethod("SetRefreshToken");
            Assert.NotNull(method);
        }

        [Fact]
        public void IJwtService_HasRevokeTokenMethod()
        {
            // Assert
            var method = typeof(IJwtService).GetMethod("RevokeToken");
            Assert.NotNull(method);
        }

        [Fact]
        public void IJwtService_HasGetUserFromJWTMethod()
        {
            // Assert
            var method = typeof(IJwtService).GetMethod("GetUserFromJWT");
            Assert.NotNull(method);
        }

        [Fact]
        public void IJwtService_HasGetTotpStatusFromJWTMethod()
        {
            // Assert
            var method = typeof(IJwtService).GetMethod("GetTotpStatusFromJWT");
            Assert.NotNull(method);
        }
    }

    public class IAuthServiceInterfaceTests
    {
        [Fact]
        public void IAuthService_IsInterface()
        {
            // Assert
            Assert.True(typeof(IAuthService).IsInterface);
        }

        [Fact]
        public void IAuthService_HasLoginMethod()
        {
            // Assert
            var method = typeof(IAuthService).GetMethod("Login");
            Assert.NotNull(method);
        }

        [Fact]
        public void IAuthService_HasRegisterMethod()
        {
            // Assert
            var method = typeof(IAuthService).GetMethod("Register");
            Assert.NotNull(method);
        }

        [Fact]
        public void IAuthService_HasRevokeTokenMethod()
        {
            // Assert
            var method = typeof(IAuthService).GetMethod("RevokeToken");
            Assert.NotNull(method);
        }

        [Fact]
        public void IAuthService_HasGetQrCodeMethod()
        {
            // Assert
            var method = typeof(IAuthService).GetMethod("GetQrCode");
            Assert.NotNull(method);
        }

        [Fact]
        public void IAuthService_HasValidateQrCodeMethod()
        {
            // Assert
            var method = typeof(IAuthService).GetMethod("ValidateQrCode");
            Assert.NotNull(method);
        }

        [Fact]
        public void IAuthService_HasCurrentUserProperty()
        {
            // Assert
            var property = typeof(IAuthService).GetProperty("CurrentUser");
            Assert.NotNull(property);
        }
    }

    public class ITotpServiceInterfaceTests
    {
        [Fact]
        public void ITotpService_IsInterface()
        {
            // Assert
            Assert.True(typeof(ITotpService).IsInterface);
        }

        [Fact]
        public void ITotpService_HasGenerateBase32SecretMethod()
        {
            // Assert
            var method = typeof(ITotpService).GetMethod("GenerateBase32Secret");
            Assert.NotNull(method);
        }

        [Fact]
        public void ITotpService_HasGenerateQrCodeMethod()
        {
            // Assert
            var method = typeof(ITotpService).GetMethod("GenerateQrCode");
            Assert.NotNull(method);
        }

        [Fact]
        public void ITotpService_HasGetSecretFromQrCodeMethod()
        {
            // Assert
            var method = typeof(ITotpService).GetMethod("GetSecretFromQrCode");
            Assert.NotNull(method);
        }

        [Fact]
        public void ITotpService_HasValidateTotpMethod()
        {
            // Assert
            var method = typeof(ITotpService).GetMethod("ValidateTotp");
            Assert.NotNull(method);
        }
    }
}
