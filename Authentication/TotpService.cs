using Microsoft.Extensions.Caching.Distributed;
using OtpNet;
using System.Globalization;
using System.Web;

namespace EcommerceWebApi.Authentication
{
    public class TotpService : ITotpService
    {
        // Redis-backed distributed cache replaces static in-memory timeWindowUsedCurrent for TOTP replay protection (cz-dotnet-1004)
        private readonly IDistributedCache _distributedCache;
        private const string TimeWindowCacheKey = "TotpService:TimeWindowUsedCurrent";

        private const string issuer = "EcommerceWebApi";

        public TotpService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public string GenerateBase32Secret()
        {
            var secret = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secret);
        }

        public string? GenerateQrCode(string? base32Secret, string? username)
        {
            // Validate inputs
            if (username == null || base32Secret == null)
            {
                return null;
            }

            try
            {
                // Generate URI for the QR Code
                var uriString = new OtpUri(OtpType.Totp, base32Secret, username, issuer).ToString();
                return uriString;
            }
            catch
            {
                return null;
            }
        }

        public string? GetSecretFromQrCode(string qrCode)
        {
            // Return null if validate failed
            if (ValidateAndExtractSecret(uriString: qrCode, out string? secret))
            {
                return secret;
            }
            else
            {
                return null;
            }
        }

        private static bool ValidateAndExtractSecret(string uriString, out string? secret)
        {
            // otpauth://totp/{issuer}:{username}?secret={secret}&issuer={issuer}&algorithm=SHA1&digits=6&period=30
            secret = null!;

            if (
                Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri)
                && uri.Scheme == "otpauth"
                && uri.Host == "totp"
            )
            {
                var queryParameters = HttpUtility.ParseQueryString(uri.Query);
                if (queryParameters["secret"] != null && queryParameters["issuer"] == issuer)
                {
                    secret = queryParameters["secret"];
                    return true;
                }
            }

            return false;
        }

        public bool ValidateTotp(string? base32Secret, string totp)
        {
            try
            {
                var secret = Base32Encoding.ToBytes(base32Secret);

                // Get exact time for TOTP
                DateTime exactTime = GetNistTime();

                // Validate TOTP
                var correction = new TimeCorrection(exactTime);
                var totpValidator = new Totp(secret, timeCorrection: correction);
                bool verify = totpValidator.VerifyTotp(
                    totp,
                    out long timeWindowUsed,
                    VerificationWindow.RfcSpecifiedNetworkDelay
                );

                // Check if TOTP has been used - use Redis distributed cache instead of static in-memory field (cz-dotnet-1004)
                var cachedWindowStr = _distributedCache.GetString(TimeWindowCacheKey);
                long timeWindowUsedCurrent = cachedWindowStr != null ? long.Parse(cachedWindowStr) : 0L;

                if (timeWindowUsedCurrent == timeWindowUsed)
                {
                    return false;
                }

                // Persist the used time window to Redis so all pods share replay protection state (cz-dotnet-1004)
                _distributedCache.SetString(TimeWindowCacheKey, timeWindowUsed.ToString(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                return verify;
            }
            catch
            {
                throw;
            }
        }

        public static DateTime GetNistTime()
        {
            // Get UTC time from the response header of request to the configured time server URL
            var timeServerUrl = Environment.GetEnvironmentVariable("TIME_SERVER_URL") ?? "http://www.google.com";
            using var httpClient = new HttpClient();
            try
            {
                using var response = httpClient.GetAsync(timeServerUrl).Result;
                if (response.IsSuccessStatusCode && response.Headers.Date != null)
                {
                    return DateTime.ParseExact(
                        response.Headers.Date.Value.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"),
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
                    );
                }
                else
                {
                    throw new Exception("Failed to get exact time for the TOTP");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get exact time for the TOTP", ex);
            }
        }
    }
}
