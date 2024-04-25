using System.Data;
using Application.Interfaces;
using Dapper;
using Domain.Contracts.Requests;
using Domain.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

public class ClientsRepository : IClientsRepository
{
    private readonly string _connectionString;

    public ClientsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<int> CreateNewClient(NewClientRequest clientData)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql =
            "insert into clients (name, document_id, phone_number, observation) OUTPUT INSERTED.ID values (@Name, @DocumentId, @PhoneNumber, @Observation);";
        var queryParams = new DynamicParameters();
        queryParams.Add("Name", clientData.Name, DbType.String);
        queryParams.Add("DocumentId", clientData.DocumentId, DbType.String);
        queryParams.Add("PhoneNumber", clientData.PhoneNumber, DbType.String);
        queryParams.Add("Observation", clientData.Observation, DbType.String);

        return await connection.ExecuteScalarAsync<int>(sql, queryParams);
    }

    public async Task<ClientDto?> GetClientById(int clientId)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql = "select a.name, a.document_id as DocumentId, a.phone_number as PhoneNumber, a.observation from clients a where id = @Id;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Id", clientId, DbType.Int32);

        return await connection.QuerySingleOrDefaultAsync<ClientDto>(sql, queryParams);
    }

    public async Task<int> DeleteClientById(int clientId)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql = "delete from clients where id = @Id;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Id", clientId, DbType.Int32);

        return await connection.ExecuteAsync(sql, queryParams);
    }

    public async Task<int> UpdateClientById(ClientDto clientData)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql =
            "update clients set name = @Name, phone_number = @PhoneNumber, document_id = @DocumentId, observation = @Observation where id = @Id;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Id", clientData.Id, DbType.Int32);
        queryParams.Add("Name", clientData.Name, DbType.String);
        queryParams.Add("PhoneNumber", clientData.PhoneNumber, DbType.String);
        queryParams.Add("DocumentId", clientData.DocumentId, DbType.String);
        queryParams.Add("Observation", clientData.Observation, DbType.String);

        return await connection.ExecuteAsync(sql, queryParams);
    }

    public async Task<IEnumerable<ClientDto>> GetAllClients()
    {
        await using var connection = new SqlConnection(_connectionString);
        var sql = "select a.id, a.name, a.document_id as DocumentId, a.phone_number as PhoneNumber, a.observation from clients a;";

        return await connection.QueryAsync<ClientDto>(sql);
    }
}