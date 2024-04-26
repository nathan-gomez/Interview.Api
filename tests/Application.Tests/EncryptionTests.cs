using System.Buffers.Text;
using Application.Interfaces;
using Application.Utils;
using Microsoft.Extensions.Configuration;

namespace Application.Tests;

public class EncryptionTests
{
    private readonly IEncryption _sut;
    private const string GuidToken = "t2VgcbfyxDEVSbsMDVZyeMOS+7A5RNxrnWPMHiJrY9j8w9dgEXAv9m+CiEl8M24fkmDdxrt9nHIy7kxxsotziQ==";
    private const string TestPlainText1 = "Test Text";
    private const string TestPlainText2 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
    private const string TestPlainText3 = "6bcd52f1-8b18-4d82-9acb-3fe3b4125ff6";

    public EncryptionTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "EncryptionKey", "PJC7HnliwcxXw4FM8Ep3sX9NIL3R5CZnDvp8IyyCSlg=" }
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
        _sut = new Encryption(configuration);
    }

    [Fact]
    public void Encryption_Should_Return_ValidBase64()
    {
        var encryptedString = _sut.EncryptString(TestPlainText1);
        var isValid = Base64.IsValid(encryptedString);

        Assert.True(isValid);
    }

    [Fact]
    public void Encryption_Should_Throw_IfString_Invalid_Or_Null()
    {
        Assert.Throws<ArgumentException>(() => _sut.EncryptString(string.Empty));
    }

    [Fact]
    public void Decryption_Should_Return_ValidGuid()
    {
        var token = _sut.DecryptString(GuidToken);
        var result = Guid.TryParse(token, out _);

        Assert.True(result);
    }

    [Fact]
    public void Decryption_Should_Throw_IfString_IsNull()
    {
        Assert.Throws<ArgumentException>(() => _sut.DecryptString(string.Empty));
    }

    [Theory]
    [InlineData(TestPlainText1)]
    [InlineData(TestPlainText2)]
    [InlineData(TestPlainText3)]
    public void Decryption_Should_Return_Same_String_Encrypted(string plainText)
    {
        var encryptedString = _sut.EncryptString(plainText);
        var decryptedString = _sut.DecryptString(encryptedString);

        Assert.Equal(plainText, decryptedString);
    }
}