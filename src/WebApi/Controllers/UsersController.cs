using Application.Interfaces;
using Domain.Contracts.Requests;
using Domain.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public sealed class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;

    public UsersController(ILogger<UsersController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// Login the user.
    /// </summary>
    /// <param name="request">The account info of the user.</param>
    /// <response code="200">Ok response if successful.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        _logger.LogInformation("Starting Login process for user: {@user}.", request.Username);
        try
        {
            var response = await _userService.Login(request);

            return response.Match<ActionResult>(
                token =>
                {
                    var options = new CookieOptions
                    {
                        HttpOnly = true,
                        Path = "/",
                        Expires = token.Expiration
                    };
                    Response.Cookies.Append("session_token", token.Id, options);

                    _logger.LogInformation("User login successful: {@user}", request.Username);
                    return Ok();
                },
                notFound =>
                {
                    _logger.LogInformation("No user found with username: {@user}", request.Username);
                    return NoContent();
                },
                validation =>
                {
                    _logger.LogInformation("Invalid password for user: {@user}", request.Username);
                    return StatusCode(StatusCodes.Status401Unauthorized, new ErrorResponse("Invalid password"));
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has ocurred in Login method");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Hash a password.
    /// </summary>
    /// <param name="pass">Password to hash</param>
    /// <response code="200">Ok response if successful.</response>
    [HttpPost("create-pass/{pass}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePassword(string pass)
    {
        try
        {
            var response = await Task.Run(() => _userService.CreatePassword(pass));
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has ocurred in CreatePassword method");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message));
        }
    }
}