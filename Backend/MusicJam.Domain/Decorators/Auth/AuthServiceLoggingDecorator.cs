using Microsoft.Extensions.Logging;
using MusicJam.Domain.Models.Auth;
using MusicJam.Domain.Models.Common;
using MusicJam.Domain.Services.AuthServices;

namespace MusicJam.Domain.Decorators.Auth;

internal class AuthServiceLoggingDecorator(
    IAuthService authService,
    ILogger<AuthServiceLoggingDecorator> logger) : IAuthService
{
    public async Task<Result<string, IAuthService.AuthError>> Login(AuthCredentials credentials)
    {
        logger.LogInformation(
            "Start login user with email = {Email}",
            credentials.Email);

        var loginResult = await authService.Login(credentials);
        if (loginResult.IsFailure)
        {
            logger.LogInformation(
                "Login user with email = {Email} finished with fault = {StatusCode}",
                credentials.Email,
                loginResult.Fault);

            return loginResult.Fault;
        }

        var token = loginResult.GetValueOrThrow();

        logger.LogInformation(
            "Login user with email = {Email} finished successfully. Access token = {Token}",
            credentials.Email,
            token);

        return loginResult.Value;
    }
}