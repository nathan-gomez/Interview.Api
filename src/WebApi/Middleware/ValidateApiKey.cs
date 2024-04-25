using System.Net;

namespace WebApi.Middleware;

public sealed class ValidateApiKey : IMiddleware
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ValidateApiKey> _logger;

    public ValidateApiKey(ILogger<ValidateApiKey> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue("x-api-key", out var apiKey))
        {
            var key = _configuration.GetSection("ApiKey").Value;
            if (!apiKey.Equals(key))
            {
                _logger.LogWarning("ApiKey doesn't match");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await next(context);
            return;
        }

        _logger.LogWarning("No ApiKey provided");
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}