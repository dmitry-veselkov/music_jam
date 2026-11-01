using System.IdentityModel.Tokens.Jwt;
using Shouldly;

namespace Tests.FunctionalTests.Extensions;

public static class StringExtensions
{
    public static void ShouldBeValidAccessToken(this string token)
    {
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).ShouldBeTrue();
    }
}
