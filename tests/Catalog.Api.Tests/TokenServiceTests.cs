using Catalog.Api.Services;
using Microsoft.Extensions.Configuration;

namespace Catalog.Api.Tests;

public class TokenServiceTests
{
    private static TokenService CreaTokenService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "Chiave-Di-Test-Solo-Per-Xunit-Non-Usare-In-Produzione",
                ["Jwt:Issuer"] = "CinemaBooking",
                ["Jwt:Audience"] = "CinemaBookingApi",
                ["Jwt:ExpiryMinutes"] = "60"
            }).Build();

        return new TokenService(config);
    }

    [Fact]
    public void GeneraToken_RestituisceUnJwtNonVuoto()
    {
        var token = CreaTokenService().GeneraToken("user");
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length);
    }
}