namespace BuildingBlocks.Application.Security;

public interface IHasher
{
    string Hash(string input, HashAlgorithmType algorithm = HashAlgorithmType.SHA256);
    bool Verify(string input, string hash, HashAlgorithmType algorithm = HashAlgorithmType.SHA256);
    string HmacHash(string input, string key, HashAlgorithmType algorithm = HashAlgorithmType.SHA256);
    bool HmacVerify(string input, string hash, string key, HashAlgorithmType algorithm = HashAlgorithmType.SHA256);
}
