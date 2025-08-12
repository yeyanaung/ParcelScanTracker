using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Helper class for AES encryption and decryption.
/// Provides static methods to securely encrypt and decrypt strings.
/// Note: Replace the hardcoded key and IV with secure values in production.
/// </summary>
public static class CryptoHelper
{
    // AES key (32 bytes) and IV (16 bytes) for encryption.
    // WARNING: Do not use hardcoded keys/IVs in production.
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("FIS32CharacterintEncryptionKey!!").Take(32).ToArray();
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("FIS16CharInitVect").Take(16).ToArray(); // 16 chars

    /// <summary>
    /// Encrypts a plain text string using AES and returns a Base64-encoded cipher text.
    /// </summary>
    public static string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
            sw.Write(plainText);

        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// Decrypts a Base64-encoded cipher text string using AES and returns the original plain text.
    /// </summary>
    public static string Decrypt(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;
        aes.Padding = PaddingMode.PKCS7; // Explicitly set padding mode

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}
