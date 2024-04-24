using Domain.Contracts.Requests;

namespace Application.Interfaces;

public interface IClientsService
{
    Task<int> NewClient(NewClientRequest request);
}