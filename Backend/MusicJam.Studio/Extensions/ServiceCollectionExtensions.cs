using MusicJam.Studio.Services.AuthService;
using MusicJam.Studio.Services.GameService;
using MusicJam.Studio.Services.PackService;
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
            .AddScoped<IQuestionsService, QuestionsService>()
            .AddScoped<IPacksService, PacksService>()
            .AddScoped<IGameService, GameService>();

        return services;
    }

    public static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services
            .AddScoped<IUserStorage, UserStorage>()
            .AddScoped<ITrackStorage, TrackStorage>()
            .AddScoped<IQuestionStorage, QuestionStorage>()
            .AddScoped<IRoundStorage, RoundStorage>()
            .AddScoped<IPackStorage, PackStorage>()
            .AddScoped<IGameStorage, GameStorage>();
        return services;
    }
}
