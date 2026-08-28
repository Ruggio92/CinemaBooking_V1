using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Catalog.Api.Tests;

public static class AuthTestHelper
{
    public static string GeneraTokenDiTest()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Chiave-Di-Test-Solo-Per-Xunit-Non-Usare-In-Produzione"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(ClaimTypes.Name, "test") };

        var token = new JwtSecurityToken(
            issuer: "CinemaBooking",
            audience: "CinemaBookingApi",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}