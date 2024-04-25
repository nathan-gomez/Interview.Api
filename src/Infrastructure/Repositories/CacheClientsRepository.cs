using System.Diagnostics;
using Application.Interfaces;
using Domain.Contracts.Requests;
using Domain.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public sealed class CacheClientsRepository : IClientsRepository
{
    private readonly IClientsRepository _clientsRepository;
    private readonly ILogger<IClientsRepository> _logger;
    private readonly IMemoryCache _cache;

    public CacheClientsRepository(IClientsRepository clientsRepository, IMemoryCache cache, ILogger<IClientsRepository> logger)
    {
        _clientsRepository = clientsRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<int> CreateNewClient(NewClientRequest clientData)
    {
        var response = await _clientsRepository.CreateNewClient(clientData);
        if (response > 0)
        {
            _cache.Remove("ClientList");
        }

        return response;
    }

    public async Task<ClientDto?> GetClientById(int clientId)
    {
        if (_cache.TryGetValue($"ClientById_{clientId}", out ClientDto? cacheResponse))
        {
            _logger.LogWarning("GetClientById returned client {@id} from cache", clientId);
            return cacheResponse;
        }

        var response = await _clientsRepository.GetClientById(clientId);
        if (response != null)
        {
            _cache.Set($"ClientById_{clientId}", response, TimeSpan.FromMinutes(30));
        }

        return response;
    }

    public async Task<int> DeleteClientById(int clientId)
    {
        var response = await _clientsRepository.DeleteClientById(clientId);
        if (response > 0)
        {
            _cache.Remove($"ClientById_{clientId}");
            _cache.Remove("ClientList");
        }

        return response;
    }

    public async Task<int> UpdateClientById(ClientDto clientData)
    {
        var response = await _clientsRepository.UpdateClientById(clientData);
        if (response > 0)
        {
            _cache.Remove($"ClientById_{clientData.Id}");
            _cache.Remove("ClientList");
        }

        return response;
    }

    public async Task<IEnumerable<ClientDto>> GetAllClients()
    {
        if (_cache.TryGetValue("ClientList", out IEnumerable<ClientDto> cacheResponse))
        {
            _logger.LogWarning("GetAllClients returned client list from cache");
            return cacheResponse;
        }

        var response = await _clientsRepository.GetAllClients();
        var responseArray = response.ToArray();

        if (responseArray.Length > 0)
        {
            _cache.Set("ClientList", responseArray, TimeSpan.FromMinutes(30));
        }

        return responseArray;
    }
}