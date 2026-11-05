using System;
using EcommerceWebApi.Authentication;
using Xunit;

namespace EcommerceWebApi.Authentication.Tests
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
        public void GenerateQrCode_WithValidInputs_ReturnsNonNullString()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();
            var username = "testuser";

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, username);

            // Assert
            Assert.NotNull(qrCode);
            Assert.NotEmpty(qrCode);
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
            // Act
            var qrCode = _totpService.GenerateQrCode(null, "testuser");

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
        public void GenerateQrCode_ReturnsOtpAuthUri()
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
        public void GetSecretFromQrCode_WithInvalidUri_ReturnsNull()
        {
            // Act
            var result = _totpService.GetSecretFromQrCode("not-a-valid-uri");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetSecretFromQrCode_WithWrongScheme_ReturnsNull()
        {
            // Act
            var result = _totpService.GetSecretFromQrCode("https://example.com?secret=ABC&issuer=EcommerceWebApi");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetSecretFromQrCode_WithWrongIssuer_ReturnsNull()
        {
            // Arrange
            var wrongIssuerUri = "otpauth://totp/WrongIssuer:testuser?secret=ABCDEFGH&issuer=WrongIssuer";

            // Act
            var result = _totpService.GetSecretFromQrCode(wrongIssuerUri);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetSecretFromQrCode_WithEmptyString_ReturnsNull()
        {
            // Act
            var result = _totpService.GetSecretFromQrCode("");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ValidateTotp_WithNullSecret_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _totpService.ValidateTotp(null, "123456"));
        }

        [Fact]
        public void ValidateTotp_WithInvalidBase32Secret_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _totpService.ValidateTotp("not-valid-base32!!!", "123456"));
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
            Assert.Contains(username, qrCode);
        }

        [Fact]
        public void GenerateQrCode_ContainsSecret()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, "testuser");

            // Assert
            Assert.NotNull(qrCode);
            Assert.Contains(secret, qrCode);
        }

        [Fact]
        public void GetSecretFromQrCode_RoundTrip_ReturnsOriginalSecret()
        {
            // Arrange
            var originalSecret = _totpService.GenerateBase32Secret();
            var qrCode = _totpService.GenerateQrCode(originalSecret, "testuser");

            // Act
            var extractedSecret = _totpService.GetSecretFromQrCode(qrCode!);

            // Assert
            Assert.Equal(originalSecret, extractedSecret);
        }
    }
}
