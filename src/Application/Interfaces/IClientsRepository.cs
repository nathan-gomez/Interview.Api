using Domain.Contracts.Requests;

namespace Application.Interfaces;

public interface IClientsRepository
{
    Task<int> CreateNewClient(NewClientRequest clientData);
}