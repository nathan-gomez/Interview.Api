using Domain.Contracts.Requests;
using Domain.Dtos;

namespace Application.Interfaces;

public interface IClientsRepository
{
    Task<int> CreateNewClient(NewClientRequest clientData);
    Task<ClientDto?> GetClientById(int clientId);
    Task<int> DeleteClientById(int clientId);
}