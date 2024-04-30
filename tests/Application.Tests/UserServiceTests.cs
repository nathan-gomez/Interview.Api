using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Domain.Contracts;
using Domain.Contracts.Requests;
using Domain.Dtos;
using Microsoft.Extensions.Configuration;
using Moq;
using OneOf.Types;

namespace Application.Tests;

public class UserServiceTests
{
    private IUserService _sut;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    public UserServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "EncryptionKey", "PJC7HnliwcxXw4FM8Ep3sX9NIL3R5CZnDvp8IyyCSlg=" }
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
        IEncryption encryption = new Encryption(configuration);

        _sut = new UserService(_userRepositoryMock.Object, encryption);
    }

    [Fact]
    public async Task Login_ShouldReturnValidToken_WithValidCredentials()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "User 1",
            Password = "a.123456",
        };
        var userDto = new UserDto
        {
            Username = "User 1",
            Password = "$2a$13$t/klZkAzjOQnrOIyPGVtxucArJjOu7Jx2kb8ji1YaRTjFl9XvQ4ny"
        };
        _userRepositoryMock.Setup(x => x.GetUserPassword(request.Username)).ReturnsAsync(userDto);

        // Act
        var response = await _sut.Login(request);

        //Assert
        Assert.IsType<TokenDto>(response.Value);
    }

    [Fact]
    public async Task Login_ShouldReturnInvalidPassword_WithInvalidCredentials()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "User 1",
            Password = "12345678",
        };
        var userDto = new UserDto
        {
            Username = "User 1",
            Password = "$2a$13$t/klZkAzjOQnrOIyPGVtxucArJjOu7Jx2kb8ji1YaRTjFl9XvQ4ny"
        };
        _userRepositoryMock.Setup(x => x.GetUserPassword(request.Username)).ReturnsAsync(userDto);

        // Act
        var response = await _sut.Login(request);

        //Assert
        Assert.Equal(UserValidationEnum.InvalidPassword, response.Value);
    }

    [Fact]
    public async Task Login_ShouldReturnNone_IfUserNotFound()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "User 1",
            Password = "12345678",
        };
        _userRepositoryMock.Setup(x => x.GetUserPassword(request.Username)).ReturnsAsync(() => null);

        // Act
        var response = await _sut.Login(request);

        //Assert
        Assert.IsType<None>(response.Value);
    }
}