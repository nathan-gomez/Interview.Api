using Domain.Contracts.Requests;
using Domain.Dtos;

namespace Application.Interfaces;

public interface IClientsService
{
    Task<int> NewClient(NewClientRequest request);
    Task<ClientDto?> GetClientById(int clientId);
}