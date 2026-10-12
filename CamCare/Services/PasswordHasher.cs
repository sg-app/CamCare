using System.Security.Cryptography;

namespace CamCare.Services
{
    /// <summary>
    /// Einfacher Passwort-Hash auf Basis von PBKDF2 (Rfc2898DeriveBytes) mit zufälligem Salt.
    /// Gespeichertes Format: v1.{Iterationen}.{SaltBase64}.{HashBase64}
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
            return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            var parts = storedHash.Split('.');
            if (parts.Length != 4 || !string.Equals(parts[0], "v1", StringComparison.Ordinal))
                return false;

            if (!int.TryParse(parts[1], out var iterations) || iterations <= 0)
                return false;

            byte[] salt;
            byte[] expectedHash;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedHash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
        }
    }
}
