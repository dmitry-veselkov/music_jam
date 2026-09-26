using Microsoft.Extensions.DependencyInjection;
using MusicJam.Domain.Decorators.Auth;
using MusicJam.Domain.Services.AuthServices;

namespace MusicJam.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IAuthService, AuthService>()
            .Decorate<IAuthService, AuthServiceLoggingDecorator>();

        return services;
    }
}