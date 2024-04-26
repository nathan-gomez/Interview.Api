using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Utils;

public sealed class Encryption : IEncryption
{
    private readonly IConfiguration _configuration;

    public Encryption(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string DecryptString(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            throw new ArgumentException($"Parameter {nameof(encryptedText)} cannot be empty");
        }

        var key = _configuration.GetSection("EncryptionKey").Value;
        if (string.IsNullOrEmpty(key))
        {
            throw new Exception("EncryptionKey not defined");
        }

        var keyBytes = Convert.FromBase64String(key);
        var combinedMessage = Convert.FromBase64String(encryptedText);

        using var aes = new AesGcm(keyBytes, AesGcm.TagByteSizes.MaxSize);

        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        Array.Copy(combinedMessage, nonce, nonce.Length);

        var ciphertext = new byte[combinedMessage.Length - nonce.Length - AesGcm.TagByteSizes.MaxSize];
        Array.Copy(combinedMessage, nonce.Length, ciphertext, 0, ciphertext.Length);

        var tag = new byte[AesGcm.TagByteSizes.MaxSize];
        Array.Copy(combinedMessage, nonce.Length + ciphertext.Length, tag, 0, tag.Length);

        byte[] decryptedText = new byte[ciphertext.Length];
        aes.Decrypt(nonce, ciphertext, tag, decryptedText);

        return Encoding.UTF8.GetString(decryptedText);
    }

    public string EncryptString(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            throw new ArgumentException($"Parameter {nameof(plainText)} cannot be empty");
        }

        var key = _configuration.GetSection("EncryptionKey").Value;
        if (string.IsNullOrEmpty(key))
        {
            throw new Exception("EncryptionKey not defined");
        }

        byte[] byteKey = Convert.FromBase64String(key);
        byte[] byteText = Encoding.UTF8.GetBytes(plainText);

        using var aes = new AesGcm(byteKey, AesGcm.TagByteSizes.MaxSize);

        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        RandomNumberGenerator.Fill(nonce);
        var ciphertext = new byte[byteText.Length];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];

        aes.Encrypt(nonce, byteText, ciphertext, tag);

        var combinedMessage = new byte[nonce.Length + ciphertext.Length + tag.Length];
        Array.Copy(nonce, 0, combinedMessage, 0, nonce.Length);
        Array.Copy(ciphertext, 0, combinedMessage, nonce.Length, ciphertext.Length);
        Array.Copy(tag, 0, combinedMessage, nonce.Length + ciphertext.Length, tag.Length);

        return Convert.ToBase64String(combinedMessage);
    }
}