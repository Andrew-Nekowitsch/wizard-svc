using System.Security.Cryptography;

namespace Utilities;

public class AccountUtilities
{

    public static string HashPassword(string password)
    {
        // Generate a 128-bit salt using a secure PRNG
        byte[] salt = RandomNumberGenerator.GetBytes(16); // 128 bits

        // Use PBKDF2 to hash the password with the salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32); // 256-bit hash

        // Combine the salt and hash as base64 for storage
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
    
        public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hashToCompare = pbkdf2.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
    }
}