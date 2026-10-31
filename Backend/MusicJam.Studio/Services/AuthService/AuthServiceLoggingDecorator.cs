using MusicJam.Studio.Requests;
using MusicJam.Studio.Requests.Auth;
using Shared;

namespace MusicJam.Studio.Services.AuthService;

internal class AuthServiceLoggingDecorator(
    IAuthService authService,
    ILogger<AuthServiceLoggingDecorator> logger) : IAuthService
{
    public async Task<Result<string, IAuthService.AuthError>> Login(LoginRequest request)
    {
        logger.LogInformation(
            "Start login user with email = {Email}",
            request.Email);

        var loginResult = await authService.Login(request);
        if (loginResult.TryGetFault(out var error, out var token))
        {
            logger.LogInformation(
                "Login user with email = {Email} finished with fault = {StatusCode}",
                request.Email,
                error);

            return error;
        }

        logger.LogInformation(
            "Login user with email = {Email} finished successfully. Access token = {Token}",
            request.Email,
            token);

        return loginResult;
    }

    public async Task<Result<string, IAuthService.AuthError>> Register(RegisterRequest request)
    {
        logger.LogInformation(
            "Start register user with email = {Email}",
            request.Email);

        var registerResult = await authService.Register(request);
        if (registerResult.TryGetFault(out var error, out var token))
        {
            logger.LogInformation(
                "Register user with email = {Email} finished with fault = {StatusCode}",
                request.Email,
                error);

            return error;
        }

        logger.LogInformation(
            "Register user with email = {Email} finished successfully. Access token = {Token}",
            request.Email,
            token);

        return registerResult;
    }
}
