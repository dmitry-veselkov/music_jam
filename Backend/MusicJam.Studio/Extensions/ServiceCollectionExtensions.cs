using Amazon.S3;
using Microsoft.Extensions.Options;
using MusicJam.Studio.Configurations;
using MusicJam.Studio.Services.AuthService;
using MusicJam.Studio.Services.PacksService;
using MusicJam.Studio.Services.QuestionsService;
using MusicJam.Studio.Services.TokenService;
using MusicJam.Studio.Services.TracksService;
using MusicJam.Studio.Storages;
using MusicJam.Studio.Storages.QuestionStorage;
using MusicJam.Studio.Storages.RoundStorage;
using MusicJam.Studio.Storages.TrackStorage;
using MusicJam.Studio.Storages.UsersStorage;
using Shared.Auth;

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
            .AddScoped<ITracksService, TracksService>();

        return services;
    }

    public static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services
            .AddScoped<IUserStorage, UserStorage>()
            .AddScoped<ITrackStorage, TrackStorage>()
            .AddScoped<IQuestionStorage, QuestionStorage>()
            .AddScoped<IRoundStorage, RoundStorage>()
            .AddScoped<IPackStorage, PackStorage>();
        return services;
    }

    public static IServiceCollection AddObjectStorage(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonS3>(provider =>
        {
            var settings = provider
                .GetRequiredService<IOptions<ObjectStorageSettings>>()
                .Value;

            var config = new AmazonS3Config
            {
                ServiceURL = settings.ObjectStorageBaseUrl,
                ForcePathStyle = true
            };

            var client = new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
            return client;
        });

        return services;
    }

    public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<ObjectStorageSettings>(configuration.GetSection(nameof(ObjectStorageSettings)))
            .Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        return services;
    }
}
