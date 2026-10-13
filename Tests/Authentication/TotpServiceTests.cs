using System;
using System.Collections.Generic;
using Xunit;
using EcommerceWebApi.Authentication;

namespace EcommerceWebApi.Tests.Authentication
{
    public class TotpServiceTests
    {
        private readonly TotpService _totpService;

        public TotpServiceTests()
        {
            _totpService = new TotpService();
        }

        [Fact]
        public void GenerateBase32Secret_ReturnsNonNullString()
        {
            // Act
            var secret = _totpService.GenerateBase32Secret();

            // Assert
            Assert.NotNull(secret);
            Assert.NotEmpty(secret);
        }

        [Fact]
        public void GenerateBase32Secret_ReturnsDifferentValuesEachTime()
        {
            // Act
            var secret1 = _totpService.GenerateBase32Secret();
            var secret2 = _totpService.GenerateBase32Secret();

            // Assert
            Assert.NotEqual(secret1, secret2);
        }

        [Fact]
        public void GenerateBase32Secret_ReturnsValidBase32String()
        {
            // Act
            var secret = _totpService.GenerateBase32Secret();

            // Assert - Base32 characters are A-Z and 2-7
            Assert.Matches("^[A-Z2-7]+=*$", secret);
        }

        [Fact]
        public void GenerateQrCode_WithValidInputs_ReturnsUri()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();
            var username = "testuser";

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, username);

            // Assert
            Assert.NotNull(qrCode);
            Assert.StartsWith("otpauth://totp/", qrCode);
        }

        [Fact]
        public void GenerateQrCode_WithNullUsername_ReturnsNull()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, null);

            // Assert
            Assert.Null(qrCode);
        }

        [Fact]
        public void GenerateQrCode_WithNullSecret_ReturnsNull()
        {
            // Arrange
            var username = "testuser";

            // Act
            var qrCode = _totpService.GenerateQrCode(null, username);

            // Assert
            Assert.Null(qrCode);
        }

        [Fact]
        public void GenerateQrCode_WithBothNull_ReturnsNull()
        {
            // Act
            var qrCode = _totpService.GenerateQrCode(null, null);

            // Assert
            Assert.Null(qrCode);
        }

        [Fact]
        public void GetSecretFromQrCode_WithValidQrCode_ReturnsSecret()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();
            var qrCode = _totpService.GenerateQrCode(secret, "testuser");

            // Act
            var extractedSecret = _totpService.GetSecretFromQrCode(qrCode!);

            // Assert
            Assert.NotNull(extractedSecret);
            Assert.Equal(secret, extractedSecret);
        }

        [Fact]
        public void GetSecretFromQrCode_WithInvalidQrCode_ReturnsNull()
        {
            // Act
            var secret = _totpService.GetSecretFromQrCode("invalid-qr-code");

            // Assert
            Assert.Null(secret);
        }

        [Fact]
        public void GetSecretFromQrCode_WithEmptyString_ReturnsNull()
        {
            // Act
            var secret = _totpService.GetSecretFromQrCode("");

            // Assert
            Assert.Null(secret);
        }

        [Fact]
        public void GetSecretFromQrCode_WithWrongScheme_ReturnsNull()
        {
            // Arrange
            var invalidUri = "https://example.com?secret=ABCDEFGH&issuer=EcommerceWebApi";

            // Act
            var secret = _totpService.GetSecretFromQrCode(invalidUri);

            // Assert
            Assert.Null(secret);
        }

        [Fact]
        public void GetSecretFromQrCode_WithWrongIssuer_ReturnsNull()
        {
            // Arrange
            var wrongIssuerUri = "otpauth://totp/WrongIssuer:testuser?secret=ABCDEFGH&issuer=WrongIssuer";

            // Act
            var secret = _totpService.GetSecretFromQrCode(wrongIssuerUri);

            // Assert
            Assert.Null(secret);
        }

        [Fact]
        public void ValidateTotp_WithInvalidBase32Secret_ThrowsException()
        {
            // Arrange
            var invalidSecret = "not-valid-base32!!!";

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _totpService.ValidateTotp(invalidSecret, "123456"));
        }

        [Fact]
        public void GenerateQrCode_ContainsIssuer()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();
            var username = "testuser";

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, username);

            // Assert
            Assert.NotNull(qrCode);
            Assert.Contains("EcommerceWebApi", qrCode);
        }

        [Fact]
        public void GenerateQrCode_ContainsUsername()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();
            var username = "myspecialuser";

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, username);

            // Assert
            Assert.NotNull(qrCode);
            Assert.Contains("myspecialuser", qrCode);
        }

        [Fact]
        public void GenerateQrCode_ContainsSecret()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();
            var username = "testuser";

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, username);

            // Assert
            Assert.NotNull(qrCode);
            Assert.Contains(secret, qrCode);
        }
    }
}
