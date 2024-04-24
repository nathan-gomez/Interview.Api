using Application.Interfaces;
using Domain.Contracts;
using Domain.Contracts.Requests;
using Domain.Dtos;
using OneOf;
using OneOf.Types;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEncryption _encryption;

    public UserService(IUserRepository userRepository, IEncryption encryption)
    {
        _userRepository = userRepository;
        _encryption = encryption;
    }

    public async Task<OneOf<TokenDto, None, UserValidationEnum>> Login(LoginRequest request)
    {
        var userData = await _userRepository.GetUserPassword(request.Username);
        if (userData == null)
        {
            return new None();
        }

        var isPassMatching = BC.EnhancedVerify(request.Password, userData.Password);

        if (!isPassMatching)
        {
            return UserValidationEnum.InvalidPassword;
        }

        var sessionId = Guid.NewGuid();
        var sessionToken = new TokenDto
        {
            Expiration = DateTime.UtcNow.AddHours(1),
            Id = _encryption.EncryptString(sessionId.ToString())
        };

        // await _userRepository.CreateSession(sessionId, userData.Id, sessionToken.Expiration);

        return sessionToken;
    }

    public string CreatePassword(string pass)
        => BC.EnhancedHashPassword(pass, 13);
}