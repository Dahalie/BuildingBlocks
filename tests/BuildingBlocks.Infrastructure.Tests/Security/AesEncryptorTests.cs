using System.Security.Cryptography;
using BuildingBlocks.Infrastructure.Security;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Security;

public class AesEncryptorTests
{
    private static AesEncryptionOptions CreateValidOptions()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return new AesEncryptionOptions { Key = Convert.ToBase64String(key) };
    }

    [Fact]
    public void EncryptDecrypt_RoundTrip_ReturnsOriginal()
    {
        var encryptor = new AesEncryptor(CreateValidOptions());

        var plainText = "Hello, World!";
        var cipherText = encryptor.Encrypt(plainText);
        var decrypted = encryptor.Decrypt(cipherText);

        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_SameInput_ProducesDifferentOutput()
    {
        var encryptor = new AesEncryptor(CreateValidOptions());

        var cipher1 = encryptor.Encrypt("hello");
        var cipher2 = encryptor.Encrypt("hello");

        cipher1.Should().NotBe(cipher2); // random nonce each time
    }

    [Fact]
    public void Encrypt_ReturnsBase64String()
    {
        var encryptor = new AesEncryptor(CreateValidOptions());

        var cipherText = encryptor.Encrypt("test");

        var act = () => Convert.FromBase64String(cipherText);
        act.Should().NotThrow();
    }

    [Fact]
    public void Decrypt_WithDifferentKey_Throws()
    {
        var encryptor1 = new AesEncryptor(CreateValidOptions());
        var encryptor2 = new AesEncryptor(CreateValidOptions());

        var cipherText = encryptor1.Encrypt("secret data");

        var act = () => encryptor2.Decrypt(cipherText);

        act.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void Decrypt_TamperedCipherText_Throws()
    {
        var encryptor = new AesEncryptor(CreateValidOptions());
        var cipherText = encryptor.Encrypt("hello");

        var bytes = Convert.FromBase64String(cipherText);
        bytes[^1] ^= 0xFF; // tamper last byte
        var tampered = Convert.ToBase64String(bytes);

        var act = () => encryptor.Decrypt(tampered);

        act.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void Decrypt_TooShortInput_ThrowsArgumentException()
    {
        var encryptor = new AesEncryptor(CreateValidOptions());

        var tooShort = Convert.ToBase64String(new byte[10]);

        var act = () => encryptor.Decrypt(tooShort);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_InvalidKeyLength_ThrowsArgumentException()
    {
        var options = new AesEncryptionOptions { Key = Convert.ToBase64String(new byte[16]) };

        var act = () => new AesEncryptor(options);

        act.Should().Throw<ArgumentException>().WithMessage("*256 bits*");
    }

    [Fact]
    public void EncryptDecrypt_EmptyString_Works()
    {
        var encryptor = new AesEncryptor(CreateValidOptions());

        var cipherText = encryptor.Encrypt("");
        var decrypted = encryptor.Decrypt(cipherText);

        decrypted.Should().BeEmpty();
    }

    [Fact]
    public void EncryptDecrypt_UnicodeContent_Works()
    {
        var encryptor = new AesEncryptor(CreateValidOptions());

        var plainText = "Merhaba dünya! 🌍 日本語";
        var cipherText = encryptor.Encrypt(plainText);
        var decrypted = encryptor.Decrypt(cipherText);

        decrypted.Should().Be(plainText);
    }
}
