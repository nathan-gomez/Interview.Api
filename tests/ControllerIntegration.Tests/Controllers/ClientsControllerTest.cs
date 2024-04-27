using System.Net;
using System.Text.Json;
using Domain.Dtos;
using Xunit.Abstractions;

namespace ControllerIntegration.Tests.Controllers;

public class ClientsControllerTest : ApiFactory
{
    private HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public ClientsControllerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task GetAllClients_WithoutData_ShouldReturnNoContent()
    {
        // Arrange
        Thread.Sleep(1000);
        await InitialDbSetup();
        _client = AppFactory.CreateClient();
        await AuthenticateAsync(_client);

        // Act
        var response = await _client.GetAsync("/v1/clients");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllClients_WithData_ShouldReturnOk()
    {
        // Arrange
        Thread.Sleep(1000);
        await InitialDbSetup();
        await SeedClients();
        _client = AppFactory.CreateClient();
        await AuthenticateAsync(_client);

        // Act
        var response = await _client.GetAsync("/v1/clients");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.IsAssignableFrom<IEnumerable<ClientDto>>(await JsonSerializer.DeserializeAsync<IEnumerable<ClientDto>>(
            await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions));
    }

    [Fact]
    public async Task DeleteClient_ShouldReturn_NotFoundStatus_WhenClientDoesntExist()
    {
        // Arrange
        Thread.Sleep(1000);
        await InitialDbSetup();
        await SeedClients();
        _client = AppFactory.CreateClient();
        await AuthenticateAsync(_client);

        // Act
        var response = await _client.DeleteAsync("/v1/clients/100000");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}