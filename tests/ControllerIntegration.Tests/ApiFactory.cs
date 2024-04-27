using System.Text;
using Application.Interfaces;
using Dapper;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ControllerIntegration.Tests;

public class ApiFactory : IAsyncLifetime
{
    protected readonly WebApplicationFactory<Program> AppFactory;
    private readonly IContainer _dbContainer;
    private readonly IConfiguration _configuration;

    protected ApiFactory(ITestOutputHelper testOutputHelper)
    {
        var port = Random.Shared.Next(10000, 20000);

        _dbContainer = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_SA_PASSWORD", "P@ssw0rd123!")
            .WithPortBinding(port, 1433)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();

        var connString = $"Server=localhost,{port};User Id=sa;Password=P@ssw0rd123!;TrustServerCertificate=True;";
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", connString }
        };
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();

        AppFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(x =>
                {
                    x.ClearProviders();
                    x.SetMinimumLevel(LogLevel.Warning);
                    x.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(testOutputHelper));
                });

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(IDbConnection));
                    services.AddSingleton<IDbConnection>(_ => new DbConnection(_configuration));
                });
            }
        );
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    protected async Task InitialDbSetup()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        await using var connection = new SqlConnection(connectionString);

        var sql = @"CREATE TABLE users
                    (
                        id         uniqueidentifier NOT NULL,
                        username   varchar(500)     NOT NULL,
                        password   varchar(100)     NOT NULL,
                        created_at datetime2        NOT NULL,
                        PRIMARY KEY (id)
                    );
                  

                    CREATE UNIQUE INDEX idx_username ON users (username);
                   

                    CREATE TABLE sessions
                    (
                        id         uniqueidentifier NOT NULL,
                        user_id    uniqueidentifier NOT NULL,
                        expiration datetime2        NOT NULL,
                        PRIMARY KEY (id),
                        FOREIGN KEY (user_id) REFERENCES users (id)
                    );
                    

                    CREATE TABLE clients
                    (
                        id           int IDENTITY (1,1) PRIMARY KEY,
                        name         varchar(500) NOT NULL,
                        document_id  varchar(100) NOT NULL,
                        phone_number varchar(200),
                        observation varchar(max)
                    );

                    INSERT INTO users (id,username, password, created_at)
                    VALUES ('6bcd52f1-8b18-4d82-9acb-3fe3b4125ff6','Admin','$2a$13$EwEKPZ0p.a2v5r1KloDhdet5EQa7587pOZSFXKCewYcRxz9B6Px1.','2024-04-21');";

        await connection.QueryMultipleAsync(sql);
    }

    protected async Task SeedClients()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        await using var connection = new SqlConnection(connectionString);

        var sql =
            @"insert into clients (name, document_id, phone_number, observation) values ('Client 1', '123456', '0981123123', 'Some observation');
                    insert into clients (name, document_id, phone_number, observation) values ('Client 2', '123456', '0981123123', '');
                    insert into clients (name, document_id, phone_number, observation) values ('Client 3', '123456', '0981123123', 'Client 3 observation');
                    insert into clients (name, document_id, phone_number, observation) values ('Client 4', '123456', '0981123123', '');";

        await connection.QueryMultipleAsync(sql);
    }

    protected async Task AuthenticateAsync(HttpClient client)
    {
        // Setting the ApiKey
        client.DefaultRequestHeaders.Add("x-api-key", "e63fc33856450a0b4a33cbe99e1c6d30f11c17d1e8d483ad5ae28388015de725");

        // Getting the session token
        var jsonPayload = "{\"username\": \"Admin\", \"password\": \"12345678\"}";
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        await client.PostAsync("/v1/users/login", content);
    }
}