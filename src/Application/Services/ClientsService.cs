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
    {
        var client = await _clientsRepository.GetClientById(clientId);
        if (client == null)
        {
            return null;
        }

        client.Id = clientId;
        return client;
    }
}