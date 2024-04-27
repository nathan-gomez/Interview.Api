using Application.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnection, DbConnection>();
        services.AddTransient<ISessionRepository, SessionRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IClientsRepository, ClientsRepository>();
        services.Decorate<IClientsRepository, CacheClientsRepository>();

        return services;
    }
}
