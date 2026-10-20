using MusicJam.Studio.Services.AuthService;
using MusicJam.Studio.Services.QuestionService;
using MusicJam.Studio.Services.TokenService;
using MusicJam.Studio.Storages;
using MusicJam.Studio.Storages.UsersStorage;

namespace MusicJam.Studio.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthService, AuthService>()
            .Decorate<IAuthService, AuthServiceLoggingDecorator>()
            .AddScoped<ITokenService, JwtTokenService>()
            .AddScoped<IQuestionService, QuestionService>();

        return services;
    }

    public static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services
            .AddScoped<IUsersStorage, UsersStorage>()
            .AddScoped<ITrackStorage, TrackStorage>()
            .AddScoped<IQuestionStorage, QuestionStorage>()
            .AddScoped<IGameStorage, GameStorage>();
        return services;
    }
}
