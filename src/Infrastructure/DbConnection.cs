using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class DbConnection : IDbConnection
{
    private readonly string _connectionString;

    public DbConnection(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public string GetConnectionString()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new Exception("ConnectionString not defined");
        }

        return _connectionString;
    }
}