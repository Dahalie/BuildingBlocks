using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Application.Security;

namespace BuildingBlocks.Infrastructure.Security;

internal sealed class Hasher : IHasher
{
    public string Hash(string input, HashAlgorithmType algorithm = HashAlgorithmType.SHA256)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = ComputeHash(algorithm, bytes);
        return Convert.ToHexStringLower(hashBytes);
    }

    public bool Verify(string input, string hash, HashAlgorithmType algorithm = HashAlgorithmType.SHA256)
    {
        var computed = Hash(input, algorithm);
        return string.Equals(computed, hash, StringComparison.OrdinalIgnoreCase);
    }

    public string HmacHash(string input, string key, HashAlgorithmType algorithm = HashAlgorithmType.SHA256)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = ComputeHmac(algorithm, keyBytes, inputBytes);
        return Convert.ToHexStringLower(hashBytes);
    }

    public bool HmacVerify(string input, string hash, string key, HashAlgorithmType algorithm = HashAlgorithmType.SHA256)
    {
        var computed = HmacHash(input, key, algorithm);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(hash.ToLowerInvariant()));
    }

    private static byte[] ComputeHash(HashAlgorithmType algorithm, byte[] input) =>
        algorithm switch
        {
            HashAlgorithmType.SHA256 => SHA256.HashData(input),
            HashAlgorithmType.SHA384 => SHA384.HashData(input),
            HashAlgorithmType.SHA512 => SHA512.HashData(input),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
        };

    private static byte[] ComputeHmac(HashAlgorithmType algorithm, byte[] key, byte[] input) =>
        algorithm switch
        {
            HashAlgorithmType.SHA256 => HMACSHA256.HashData(key, input),
            HashAlgorithmType.SHA384 => HMACSHA384.HashData(key, input),
            HashAlgorithmType.SHA512 => HMACSHA512.HashData(key, input),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
        };
}
