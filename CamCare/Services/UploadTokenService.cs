using CamCare.Interfaces.Services;
using CamCare.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CamCare.Services
{
    /// <summary>
    /// Zustandslose HMAC-SHA256-Signierung zeitlich begrenzter Upload-Tokens.
    /// Token-Format: {RepairOrderId}.{AblaufzeitUTC}.{Base64Url-Signatur}
    /// Die Ablaufzeit ist UTC im Format "yyyyMMddHHmmss" (lexikografisch vergleichbar).
    /// Die Signatur wird über "{RepairOrderId}.{AblaufzeitUTC}" mit einem
    /// geheimen Schlüssel (UploadLink.Secret) berechnet.
    /// </summary>
    public class UploadTokenService : IUploadTokenService
    {
        private static readonly int HmacBlockSize = 64;
        private static readonly int HmacSignatureLength = 32;
        private static readonly string TokenTimeFormat = "yyyyMMddHHmmss";

        private readonly UploadLinkOptions _options;
        private readonly ILogger<UploadTokenService> _logger;

        public UploadTokenService(IOptions<UploadLinkOptions> options, ILogger<UploadTokenService> logger)
        {
            _options = options.Value;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_options.Secret) || _options.Secret.Length < 16)
                throw new InvalidOperationException(
                    "UploadLink.Secret ist nicht konfiguriert oder zu kurz (mindestens 16 Zeichen). "
                    + "Bitte über die Umgebungsvariable UploadLink__Secret setzen.");
        }

        public string CreateToken(int repairOrderId)
        {
            var expiresAt = DateTime.UtcNow
                .Add(TimeSpan.FromMinutes(_options.TokenLifetimeMinutes))
                .ToString(TokenTimeFormat);
            var payload = $"{repairOrderId}.{expiresAt}";
            var signature = HmacSha256(ToUtf8Bytes(_options.Secret), ToUtf8Bytes(payload));
            return $"{payload}.{ToBase64Url(signature)}";
        }

        public bool TryValidate(string token, out int repairOrderId)
        {
            repairOrderId = 0;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            var parts = token.Split('.');
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out var parsedId) || parsedId <= 0)
                return false;

            var expiresAt = parts[1];
            if (expiresAt.Length != TokenTimeFormat.Length || !IsDigitsOnly(expiresAt))
                return false;

            // Abgelaufene Tokens ablehnen (lexikografischer Vergleich, da UTC "yyyyMMddHHmmss").
            if (expiresAt.CompareTo(DateTime.UtcNow.ToString(TokenTimeFormat)) < 0)
            {
                _logger.LogDebug("Abgelaufenes Upload-Token für RepairOrder {RepairOrderId} verworfen.", parsedId);
                return false;
            }

            byte[] expectedSignature;
            try
            {
                expectedSignature = FromBase64Url(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            if (expectedSignature.Length != HmacSignatureLength)
                return false;

            var payload = $"{parsedId}.{expiresAt}";
            var computedSignature = HmacSha256(ToUtf8Bytes(_options.Secret), ToUtf8Bytes(payload));

            if (!CryptographicOperations.FixedTimeEquals(computedSignature, expectedSignature))
            {
                _logger.LogDebug("Ungültige Signatur eines Upload-Tokens verworfen.");
                return false;
            }

            repairOrderId = parsedId;
            return true;
        }

        /// <summary>
        /// HMAC-SHA256 (RFC 2104) auf Basis der statischen SHA256.HashData API.
        /// </summary>
        private static byte[] HmacSha256(byte[] key, byte[] message)
        {
            var keyBlock = key.Length > HmacBlockSize ? SHA256.HashData(key) : CopyOf(key, HmacBlockSize);

            var innerPad = new byte[HmacBlockSize];
            var outerPad = new byte[HmacBlockSize];
            for (var i = 0; i < HmacBlockSize; i++)
            {
                innerPad[i] = (byte)(keyBlock[i] ^ 0x36);
                outerPad[i] = (byte)(keyBlock[i] ^ 0x5c);
            }

            var innerHash = SHA256.HashData(Concat(innerPad, message));
            return SHA256.HashData(Concat(outerPad, innerHash));
        }

        private static byte[] CopyOf(byte[] source, int length)
        {
            var copy = new byte[length];
            for (var i = 0; i < Math.Min(source.Length, length); i++) copy[i] = source[i];
            return copy;
        }

        private static byte[] Concat(byte[] a, byte[] b)
        {
            var result = new byte[a.Length + b.Length];
            for (var i = 0; i < a.Length; i++) result[i] = a[i];
            for (var i = 0; i < b.Length; i++) result[a.Length + i] = b[i];
            return result;
        }

        /// <summary>
        /// UTF-8-Kodierung eines Strings in ein Byte-Array (String ist IEnumerable&lt;char&gt;).
        /// </summary>
        private static byte[] ToUtf8Bytes(string text)
        {
            var count = 0;
            foreach (var c in text)
            {
                if (c <= 0x7f) count++;
                else if (c <= 0x7ff) count += 2;
                else if (c <= 0xffff) count += 3;
                else count += 4;
            }

            var result = new byte[count];
            var index = 0;
            foreach (var c in text)
            {
                if (c <= 0x7f)
                {
                    result[index++] = (byte)c;
                }
                else if (c <= 0x7ff)
                {
                    result[index++] = (byte)(0xc0 | (c >> 6));
                    result[index++] = (byte)(0x80 | (c & 0x3f));
                }
                else if (c <= 0xffff)
                {
                    result[index++] = (byte)(0xe0 | (c >> 12));
                    result[index++] = (byte)(0x80 | ((c >> 6) & 0x3f));
                    result[index++] = (byte)(0x80 | (c & 0x3f));
                }
                else
                {
                    result[index++] = (byte)(0xf0 | (c >> 18));
                    result[index++] = (byte)(0x80 | ((c >> 12) & 0x3f));
                    result[index++] = (byte)(0x80 | ((c >> 6) & 0x3f));
                    result[index++] = (byte)(0x80 | (c & 0x3f));
                }
            }
            return result;
        }

        /// <summary>
        /// URL-sichere Base64-Kodierung ohne Padding (RFC 4648, Abschnitt 5).
        /// </summary>
        private static string ToBase64Url(byte[] data)
        {
            return Convert.ToBase64String(data)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private static byte[] FromBase64Url(string data)
        {
            var base64 = data.Replace('-', '+').Replace('_', '/');
            var padding = (4 - base64.Length % 4) % 4;
            var sb = new System.Text.StringBuilder(base64);
            for (var i = 0; i < padding; i++) sb.Append('=');
            return Convert.FromBase64String(sb.ToString());
        }

        private static bool IsDigitsOnly(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            foreach (var c in value)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
    }
}
