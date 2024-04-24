using System.Data;
using Application.Interfaces;
using Dapper;
using Domain.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<UserDto?> GetUserPassword(string username)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql = "select id, password from users where username = @Username;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Username", username, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UserDto>(sql, queryParams);
    }

    public async Task CreateSession(Guid sessionId, Guid userId, DateTime expiration)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql = "insert into sessions (id ,user_id, expiration) values (@Id, @UserId, @Expiration);";
        var queryParams = new DynamicParameters();
        queryParams.Add("Id", sessionId, DbType.Guid);
        queryParams.Add("UserId", userId, DbType.Guid);
        queryParams.Add("Expiration", expiration, DbType.DateTime2);

        await connection.ExecuteAsync(sql, queryParams);
    }
}