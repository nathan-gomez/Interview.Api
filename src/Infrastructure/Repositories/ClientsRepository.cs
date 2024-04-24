using System.Data;
using Application.Interfaces;
using Dapper;
using Domain.Contracts.Requests;
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

        var sql = "insert into clients (name, document_id, phone_number, observation) OUTPUT INSERTED.ID values (@Name, @DocumentId, @PhoneNumber, @Observation);";
        var queryParams = new DynamicParameters();
        queryParams.Add("Name", clientData.Name, DbType.String);
        queryParams.Add("DocumentId", clientData.DocumentId, DbType.String);
        queryParams.Add("PhoneNumber", clientData.PhoneNumber, DbType.String);
        queryParams.Add("Observation", clientData.Observation, DbType.String);

        return await connection.ExecuteScalarAsync<int>(sql, queryParams);
    }
}