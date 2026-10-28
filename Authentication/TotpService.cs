using OtpNet;
using Polly;
using Polly.Extensions.Http;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceWebApi.Authentication
{
    public class TotpService : ITotpService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.BadGateway)
                .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                .WaitAndRetryAsync(
                    retryCount: 4,
                    sleepDurationProvider: attempt =>
                        TimeSpan.FromMilliseconds(500 * Math.Pow(2, attempt))
                        + TimeSpan.FromMilliseconds(new Random().Next(0, 300)),
                    onRetry: (result, timespan, retryAttempt, _) =>
                    {
                        // Retry with exponential backoff and jitter
                    });

        private static readonly ConcurrentDictionary<string, long> _usedTimeWindows =
            new ConcurrentDictionary<string, long>();

        private const string issuer = "EcommerceWebApi";

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
                var query = uri.Query.TrimStart('?');
                var queryDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrEmpty(query))
                {
                    foreach (var param in query.Split('&'))
                    {
                        var idx = param.IndexOf('=');
                        if (idx > 0)
                        {
                            var key = System.Net.WebUtility.UrlDecode(param.Substring(0, idx));
                            var value = System.Net.WebUtility.UrlDecode(param.Substring(idx + 1));
                            if (key != null)
                            {
                                queryDict[key] = value ?? "";
                            }
                        }
                    }
                }
                if (queryDict.TryGetValue("secret", out string? secretValue) && secretValue != null
                    && queryDict.TryGetValue("issuer", out string? issuerValue) && issuerValue == issuer)
                {
                    secret = secretValue;
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> ValidateTotpAsync(
            string? base32Secret,
            string totp,
            CancellationToken ct = default)
        {
            try
            {
                var secret = Base32Encoding.ToBytes(base32Secret);

                // Get exact time for TOTP
                DateTime exactTime = await GetNistTimeAsync(ct).ConfigureAwait(false);

                // Validate TOTP
                var correction = new TimeCorrection(exactTime);
                var totpValidator = new Totp(secret, timeCorrection: correction);
                bool verify = totpValidator.VerifyTotp(
                    totp,
                    out long timeWindowUsed,
                    VerificationWindow.RfcSpecifiedNetworkDelay
                );

                // Check if TOTP has been used
                string key = totp;
                if (_usedTimeWindows.TryGetValue(key, out long previousWindow) && previousWindow == timeWindowUsed)
                {
                    return false;
                }
                _usedTimeWindows[key] = timeWindowUsed;

                return verify;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<DateTime> GetNistTimeAsync(CancellationToken ct = default)
        {
            // Get UTC time from the response header of request to "http://www.google.com"
            try
            {
                using var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync("http://www.google.com", ct)).ConfigureAwait(false);
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
