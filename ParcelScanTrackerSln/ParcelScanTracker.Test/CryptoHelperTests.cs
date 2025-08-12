using Xunit;
namespace ParcelScanTracker.Test
{
    /// <summary>
    /// Unit tests for the CryptoHelper class.
    /// Verifies encryption and decryption functionality.
    /// </summary>
    public class CryptoHelperTests
    {
        /// <summary>
        /// Ensures that encrypting and then decrypting a string returns the original value.
        /// </summary>
        [Fact]
        public void Encrypt_And_Decrypt_ReturnsOriginalText()
        {
            // Arrange
            var plainText = "TestPassword";

            // Act
            var encryptedText = CryptoHelper.Encrypt(plainText);
            var decryptedText = CryptoHelper.Decrypt(encryptedText);

            // Assert
            Assert.Equal(plainText, decryptedText);
        }

        /// <summary>
        /// Ensures that decrypting an invalid cipher text throws a CryptographicException.
        /// </summary>
        [Fact]
        public void Decrypt_ThrowsException_WhenInvalidCipherText()
        {
            // Arrange
            var invalidCipherText = "InvalidCipherText";

            // Act & Assert
            Assert.Throws<System.Security.Cryptography.CryptographicException>(() => CryptoHelper.Decrypt(invalidCipherText));
        }
    }
}
