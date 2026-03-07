using BuildingBlocks.Application.Security;
using BuildingBlocks.Infrastructure.Security;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Security;

public class HasherTests
{
    private readonly Hasher _hasher = new();

    [Fact]
    public void Hash_DefaultAlgorithm_ReturnsSha256Hex()
    {
        var result = _hasher.Hash("hello");

        result.Should().HaveLength(64); // SHA256 = 32 bytes = 64 hex chars
        result.Should().MatchRegex("^[0-9a-f]+$");
    }

    [Fact]
    public void Hash_SameInput_ReturnsSameHash()
    {
        var hash1 = _hasher.Hash("test");
        var hash2 = _hasher.Hash("test");

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Hash_DifferentInputs_ReturnDifferentHashes()
    {
        var hash1 = _hasher.Hash("hello");
        var hash2 = _hasher.Hash("world");

        hash1.Should().NotBe(hash2);
    }

    [Theory]
    [InlineData(HashAlgorithmType.SHA256, 64)]
    [InlineData(HashAlgorithmType.SHA384, 96)]
    [InlineData(HashAlgorithmType.SHA512, 128)]
    public void Hash_WithAlgorithm_ReturnsCorrectLength(HashAlgorithmType algorithm, int expectedLength)
    {
        var result = _hasher.Hash("test", algorithm);

        result.Should().HaveLength(expectedLength);
    }

    [Fact]
    public void Verify_CorrectHash_ReturnsTrue()
    {
        var hash = _hasher.Hash("hello");

        _hasher.Verify("hello", hash).Should().BeTrue();
    }

    [Fact]
    public void Verify_WrongInput_ReturnsFalse()
    {
        var hash = _hasher.Hash("hello");

        _hasher.Verify("wrong", hash).Should().BeFalse();
    }

    [Fact]
    public void Verify_CaseInsensitiveHash_ReturnsTrue()
    {
        var hash = _hasher.Hash("hello");
        var upperHash = hash.ToUpperInvariant();

        _hasher.Verify("hello", upperHash).Should().BeTrue();
    }

    [Fact]
    public void HmacHash_ReturnsConsistentResult()
    {
        var hash1 = _hasher.HmacHash("data", "secret");
        var hash2 = _hasher.HmacHash("data", "secret");

        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(64);
    }

    [Fact]
    public void HmacHash_DifferentKeys_ReturnDifferentHashes()
    {
        var hash1 = _hasher.HmacHash("data", "key1");
        var hash2 = _hasher.HmacHash("data", "key2");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void HmacVerify_CorrectHash_ReturnsTrue()
    {
        var hash = _hasher.HmacHash("data", "secret");

        _hasher.HmacVerify("data", hash, "secret").Should().BeTrue();
    }

    [Fact]
    public void HmacVerify_WrongKey_ReturnsFalse()
    {
        var hash = _hasher.HmacHash("data", "secret");

        _hasher.HmacVerify("data", hash, "wrong-key").Should().BeFalse();
    }

    [Theory]
    [InlineData(HashAlgorithmType.SHA256, 64)]
    [InlineData(HashAlgorithmType.SHA384, 96)]
    [InlineData(HashAlgorithmType.SHA512, 128)]
    public void HmacHash_WithAlgorithm_ReturnsCorrectLength(HashAlgorithmType algorithm, int expectedLength)
    {
        var result = _hasher.HmacHash("data", "key", algorithm);

        result.Should().HaveLength(expectedLength);
    }
}
