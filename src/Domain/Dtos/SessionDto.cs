namespace Domain.Dtos;

public class SessionDto
{
    public Guid UserId { get; init; }
    public DateTime Expiration { get; init; }
}