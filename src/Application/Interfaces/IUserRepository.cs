using Domain.Dtos;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<UserPassDto?> GetUserPassword(string username);
}