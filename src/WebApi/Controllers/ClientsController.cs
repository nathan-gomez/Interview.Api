using Application.Interfaces;
using Domain.Contracts.Requests;
using Domain.Contracts.Responses;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public sealed class ClientsController : ControllerBase
{
    private readonly ILogger<ClientsController> _logger;
    private readonly IClientsService _clientsService;

    public ClientsController(ILogger<ClientsController> logger, IClientsService clientsService)
    {
        _logger = logger;
        _clientsService = clientsService;
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="request">The data of the new client.</param>
    /// <response code="201">Ok response if created.</response>
    [HttpPost("new-client")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> NewClient(NewClientRequest request)
    {
        _logger.LogInformation("Starting NewClient method.");
        try
        {
            var newClientId = await _clientsService.NewClient(request);

            _logger.LogInformation("New client created: {@name}", request.Name);
            return CreatedAtAction(nameof(GetClientById), routeValues: new { id = newClientId }, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has ocurred in NewClient method");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Gets data of a single client.
    /// </summary>
    /// <param name="id">The client id.</param>
    /// <response code="200">The client data.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClientDto>> GetClientById([FromRoute] int id)
    {
        _logger.LogInformation("Starting GetClientById method.");
        try
        {
            var response = await _clientsService.GetClientById(id);

            if (response is null)
            {
                return NoContent();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has ocurred in GetClientById method");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Deletes a client.
    /// </summary>
    /// <param name="id">The client id.</param>
    /// <response code="200">Ok if deleted.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteClient([FromRoute] int id)
    {
        _logger.LogInformation("Starting DeleteClient method.");
        try
        {
            // string sessionId = HttpContext.Items["SessionId"]?.ToString();
            var response = await _clientsService.DeleteClientById(id);
            if (response == 0)
            {
                _logger.LogInformation("No client found with id: {@id}.", id);
                return NotFound();
            }

            _logger.LogInformation("Deleted client with id: {@id}.", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has ocurred in DeleteClient method");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Updates the client info.
    /// </summary>
    /// <param name="request">The client data to update.</param>
    /// <response code="200">Ok if successful.</response>
    [HttpPut("update-client")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateClient(ClientDto request)
    {
        _logger.LogInformation("Starting UpdateClient method for client: {@id}.", request.Id);
        try
        {
            var response = await _clientsService.UpdateClientById(request);
            if (response == 0)
            {
                _logger.LogInformation("No client found with id: {@id}.", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Updated the client with id: {@id}.", request.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has ocurred in UpdateClient method");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message));
        }
    }
}