namespace BuildingBlocks.Application.Security;

public interface IEncryptor
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
