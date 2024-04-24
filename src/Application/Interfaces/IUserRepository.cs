using Domain.Dtos;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<UserDto?> GetUserPassword(string username);
    Task CreateSession(Guid sessionId, Guid userId, DateTime expiration);
}