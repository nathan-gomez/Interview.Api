using Domain.Dtos;

namespace Application.Interfaces;

public interface ISessionRepository
{
    Task<SessionDto> GetSession(string sessionId);
    Task DeleteSession(string sessionId);
}