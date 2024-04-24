using System.Data;
using System.Net;
using Application.Interfaces;
using Dapper;
using Domain.Dtos;
using Microsoft.Data.SqlClient;


namespace WebApi.Middleware;

public sealed class ValidateSession : IMiddleware
{
    private readonly IEncryption _encryption;
    private readonly ILogger<ValidateSession> _logger;
    private readonly string _connectionString;

    public ValidateSession(IEncryption encryption, ILogger<ValidateSession> logger, IConfiguration configuration)
    {
        _encryption = encryption;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Cookies.TryGetValue("session_token", out var token))
        {
            try
            {
                var sessionId = _encryption.DecryptString(token);
                var currentSession = await GetSession(sessionId);

                if (currentSession.Expiration < DateTime.Now)
                {
                    await DeleteSession(sessionId);
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

    private async Task<SessionDto> GetSession(string sessionId)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql = "select expiration, user_id as userId from sessions where id = @Id;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Id", sessionId, DbType.String);

        var response = await connection.QueryFirstAsync<SessionDto>(sql, queryParams);

        return response;
    }

    private async Task DeleteSession(string sessionId)
    {
        await using var connection = new SqlConnection(_connectionString);

        var sql = "delete from sessions where id = @Id;";
        var queryParams = new DynamicParameters();
        queryParams.Add("Id", sessionId, DbType.Guid);

        await connection.ExecuteAsync(sql, queryParams);
    }
}