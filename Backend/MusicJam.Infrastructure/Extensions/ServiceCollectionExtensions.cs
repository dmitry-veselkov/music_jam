using Microsoft.Extensions.DependencyInjection;
using MusicJam.Domain.Services.Tokens;
using MusicJam.Domain.Storages;
using MusicJam.Infrastructure.Services;
using MusicJam.Infrastructure.Storages;

namespace MusicJam.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddStorages()
            .AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }

    private static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services.AddScoped<IUsersStorage, UsersStorage>();

        return services;
    }
}