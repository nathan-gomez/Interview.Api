using System.Data;
using Application.Interfaces;
using Dapper;
using Domain.Dtos;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<UserPassDto?> GetUserPassword(string username)
    {
        await using var connection = new SqliteConnection(_connectionString);

        var sql = "select id, password from users where username = @Username;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Username", username, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UserPassDto>(sql, queryParams);
    }
}