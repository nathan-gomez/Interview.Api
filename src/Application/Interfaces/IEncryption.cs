namespace Application.Interfaces;

public interface IEncryption
{
    string EncryptString(string plainText);
    string DecryptString(string encryptedText);
}