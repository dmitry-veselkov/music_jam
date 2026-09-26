using Microsoft.Extensions.Logging;
using MusicJam.Domain.Models.Auth;
using MusicJam.Domain.Models.Common;
using MusicJam.Domain.Services.AuthServices;

namespace MusicJam.Domain.Decorators.Auth;

internal class AuthServiceLoggingDecorator(
    IAuthService authService,
    ILogger<AuthServiceLoggingDecorator> logger) : IAuthService
{
    public Task<Result<IAuthService.AuthError>> Login(AuthCredentials credentials)
    {
        logger.LogInformation(
            "Тестовая реализация подразумевает лишь отправку ошибки {Error}",
            nameof(IAuthService.AuthError.SameUserAlreadyExists));

        return authService.Login(credentials);
    }
}