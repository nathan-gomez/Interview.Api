using Application.Interfaces;
using Domain.Contracts.Requests;
using Domain.Dtos;

namespace Application.Services;

public sealed class ClientsService : IClientsService
{
    private readonly IClientsRepository _clientsRepository;

    public ClientsService(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<int> NewClient(NewClientRequest request)
        => await _clientsRepository.CreateNewClient(request);

    public async Task<ClientDto?> GetClientById(int clientId)
        => await _clientsRepository.GetClientById(clientId);

    public async Task<int> DeleteClientById(int clientId)
        => await _clientsRepository.DeleteClientById(clientId);

    public async Task<int> UpdateClientById(ClientDto clientData)
        => await _clientsRepository.UpdateClientById(clientData);

    public async Task<IEnumerable<ClientDto>> GetAllClients()
        => await _clientsRepository.GetAllClients();
}