using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Application.Security;

namespace BuildingBlocks.Infrastructure.Security;

internal sealed class AesEncryptor : IEncryptor
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    private readonly byte[] _key;

    public AesEncryptor(AesEncryptionOptions options)
    {
        _key = Convert.FromBase64String(options.Key);

        if (_key.Length != 32)
            throw new ArgumentException("Key must be 256 bits (32 bytes) encoded as base64.");
    }

    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        var result = new byte[NonceSize + TagSize + cipherBytes.Length];
        nonce.CopyTo(result, 0);
        tag.CopyTo(result, NonceSize);
        cipherBytes.CopyTo(result, NonceSize + TagSize);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var combined = Convert.FromBase64String(cipherText);

        if (combined.Length < NonceSize + TagSize)
            throw new ArgumentException("Invalid cipher text.");

        var nonce = combined.AsSpan(0, NonceSize);
        var tag = combined.AsSpan(NonceSize, TagSize);
        var cipherBytes = combined.AsSpan(NonceSize + TagSize);

        var plainBytes = new byte[cipherBytes.Length];

        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
