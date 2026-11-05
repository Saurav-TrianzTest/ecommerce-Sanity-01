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
        public void GenerateBase32Secret_ReturnsNonEmptyString()
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

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, "testuser");

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
        public void GenerateQrCode_ReturnsOtpauthUri()
        {
            // Arrange
            var secret = _totpService.GenerateBase32Secret();

            // Act
            var qrCode = _totpService.GenerateQrCode(secret, "testuser");

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
        public void GetSecretFromQrCode_WithWrongIssuer_ReturnsNull()
        {
            // Arrange - URI with wrong issuer
            var wrongIssuerUri = "otpauth://totp/WrongIssuer:testuser?secret=TESTSECRET&issuer=WrongIssuer&algorithm=SHA1&digits=6&period=30";

            // Act
            var secret = _totpService.GetSecretFromQrCode(wrongIssuerUri);

            // Assert
            Assert.Null(secret);
        }

        [Fact]
        public void GetSecretFromQrCode_WithHttpUri_ReturnsNull()
        {
            // Arrange
            var httpUri = "https://example.com/qrcode";

            // Act
            var secret = _totpService.GetSecretFromQrCode(httpUri);

            // Assert
            Assert.Null(secret);
        }

        [Fact]
        public void ValidateTotp_WithInvalidSecret_ThrowsException()
        {
            // Arrange
            var invalidSecret = "INVALID!!!";

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _totpService.ValidateTotp(invalidSecret, "123456"));
        }
    }
}
