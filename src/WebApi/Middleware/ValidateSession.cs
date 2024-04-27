using System.Net;
using Application.Interfaces;

namespace WebApi.Middleware;

public sealed class ValidateSession : IMiddleware
{
    private readonly IEncryption _encryption;
    private readonly ILogger<ValidateSession> _logger;
    private readonly ISessionRepository _sessionRepository;

    public ValidateSession(IEncryption encryption, ILogger<ValidateSession> logger, ISessionRepository sessionRepository)
    {
        _encryption = encryption;
        _logger = logger;
        _sessionRepository = sessionRepository;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Cookies.TryGetValue("session_token", out var token))
        {
            try
            {
                var sessionId = _encryption.DecryptString(token);
                var currentSession = await _sessionRepository.GetSession(sessionId);

                if (currentSession.Expiration < DateTime.Now)
                {
                    await _sessionRepository.DeleteSession(sessionId);
                    context.Response.Cookies.Delete("session_token");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                context.Items["UserId"] = currentSession.UserId;
                context.Items["SessionId"] = sessionId;

                await next(context);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has ocurred while validating the session");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return;
            }
        }

        _logger.LogWarning("No session token provided");
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}