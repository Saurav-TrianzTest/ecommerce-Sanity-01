using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using EcommerceWebApi.Authentication;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Services;

namespace EcommerceWebApi.Authentication.Tests
{
    public class TotpServiceUnitTests
    {
        [Fact]
        public void TotpService_GenerateBase32Secret_ReturnsNonEmptyString()
        {
            // Arrange
            var service = new TotpService();

            // Act
            var result = service.GenerateBase32Secret();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TotpService_GenerateBase32Secret_ReturnsDifferentValuesEachTime()
        {
            // Arrange
            var service = new TotpService();

            // Act
            var secret1 = service.GenerateBase32Secret();
            var secret2 = service.GenerateBase32Secret();

            // Assert
            Assert.NotEqual(secret1, secret2);
        }

        [Fact]
        public void TotpService_GenerateQrCode_WithValidInputs_ReturnsUri()
        {
            // Arrange
            var service = new TotpService();
            var secret = service.GenerateBase32Secret();

            // Act
            var result = service.GenerateQrCode(secret, "testuser");

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("otpauth://totp", result);
        }

        [Fact]
        public void TotpService_GenerateQrCode_WithNullUsername_ReturnsNull()
        {
            // Arrange
            var service = new TotpService();

            // Act
            var result = service.GenerateQrCode("JBSWY3DPEHPK3PXP", null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TotpService_GenerateQrCode_WithNullSecret_ReturnsNull()
        {
            // Arrange
            var service = new TotpService();

            // Act
            var result = service.GenerateQrCode(null, "testuser");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TotpService_GenerateQrCode_WithBothNull_ReturnsNull()
        {
            // Arrange
            var service = new TotpService();

            // Act
            var result = service.GenerateQrCode(null, null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TotpService_GetSecretFromQrCode_WithValidQrCode_ReturnsSecret()
        {
            // Arrange
            var service = new TotpService();
            var secret = service.GenerateBase32Secret();
            var qrCode = service.GenerateQrCode(secret, "testuser");

            // Act
            var result = service.GetSecretFromQrCode(qrCode!);

            // Assert
            Assert.Equal(secret, result);
        }

        [Fact]
        public void TotpService_GetSecretFromQrCode_WithInvalidQrCode_ReturnsNull()
        {
            // Arrange
            var service = new TotpService();

            // Act
            var result = service.GetSecretFromQrCode("not-a-valid-qr-code");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TotpService_GetSecretFromQrCode_WithWrongIssuer_ReturnsNull()
        {
            // Arrange
            var service = new TotpService();
            var wrongIssuerQr = "otpauth://totp/WrongIssuer:testuser?secret=JBSWY3DPEHPK3PXP&issuer=WrongIssuer";

            // Act
            var result = service.GetSecretFromQrCode(wrongIssuerQr);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TotpService_GetSecretFromQrCode_WithHttpScheme_ReturnsNull()
        {
            // Arrange
            var service = new TotpService();

            // Act
            var result = service.GetSecretFromQrCode("http://example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TotpService_GetSecretFromQrCode_WithEmptyString_ReturnsNull()
        {
            // Arrange
            var service = new TotpService();

            // Act
            var result = service.GetSecretFromQrCode("");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TotpService_GetSecretFromQrCode_WithNoSecretParam_ReturnsNull()
        {
            // Arrange
            var service = new TotpService();
            var noSecretQr = "otpauth://totp/EcommerceWebApi:testuser?issuer=EcommerceWebApi";

            // Act
            var result = service.GetSecretFromQrCode(noSecretQr);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TotpService_GenerateQrCode_ContainsIssuer()
        {
            // Arrange
            var service = new TotpService();
            var secret = service.GenerateBase32Secret();

            // Act
            var result = service.GenerateQrCode(secret, "testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("EcommerceWebApi", result);
        }

        [Fact]
        public void TotpService_GenerateQrCode_ContainsUsername()
        {
            // Arrange
            var service = new TotpService();
            var secret = service.GenerateBase32Secret();

            // Act
            var result = service.GenerateQrCode(secret, "myuser");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("myuser", result);
        }

        [Fact]
        public void TotpService_RoundTrip_SecretPreserved()
        {
            // Arrange
            var service = new TotpService();
            var originalSecret = service.GenerateBase32Secret();
            var qrCode = service.GenerateQrCode(originalSecret, "testuser");

            // Act
            var extractedSecret = service.GetSecretFromQrCode(qrCode!);

            // Assert
            Assert.Equal(originalSecret, extractedSecret);
        }
    }
}
