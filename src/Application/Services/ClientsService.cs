using Application.Interfaces;
using Domain.Contracts.Requests;

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
}