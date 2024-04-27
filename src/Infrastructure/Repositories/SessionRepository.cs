using System.Data;
using Application.Interfaces;
using Dapper;
using Domain.Dtos;
using Microsoft.Data.SqlClient;
using IDbConnection = Application.Interfaces.IDbConnection;

namespace Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly string _connectionString;

    public SessionRepository(IDbConnection connection)
    {
        _connectionString = connection.GetConnectionString();
    }

    public async Task<SessionDto> GetSession(string sessionId)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql = "select expiration, user_id as userId from sessions where id = @Id;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Id", sessionId, DbType.String);

        var response = await connection.QueryFirstAsync<SessionDto>(sql, queryParams);

        return response;
    }

    public async Task DeleteSession(string sessionId)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql = "delete from sessions where id = @Id;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Id", sessionId, DbType.Guid);

        await connection.ExecuteAsync(sql, queryParams);
    }
}