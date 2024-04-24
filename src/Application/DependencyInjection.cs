using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClientsService, ClientsService>();

        services.AddSingleton<IEncryption, Encryption>();

        return services;
    }
}