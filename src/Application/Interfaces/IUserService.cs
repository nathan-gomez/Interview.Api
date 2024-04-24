using Domain.Contracts;
using Domain.Contracts.Requests;
using Domain.Dtos;
using OneOf.Types;
using OneOf;

namespace Application.Interfaces;

public interface IUserService
{
    Task<OneOf<TokenDto, None, UserValidationEnum>> Login(LoginRequest request);
    string CreatePassword(string pass);
}