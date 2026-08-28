using Catalog.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Catalog.Api.Tests;

public class CatalogApiFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "Chiave-Di-Test-Solo-Per-Xunit-Non-Usare-In-Produzione",
                ["Jwt:Issuer"] = "CinemaBooking",
                ["Jwt:Audience"] = "CinemaBookingApi",
                ["Jwt:ExpiryMinutes"] = "60",
                ["Auth:Username"] = "user",
                ["Auth:Password"] = "Password123!"
            });
        });

        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<CatalogDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<CatalogDbContext>>();

            _connection.Open();
            services.AddDbContext<CatalogDbContext>(options => options.UseSqlite(_connection).ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            db.Database.EnsureCreated();

        });
    }

    public new void Dispose()
    {
        _connection.Dispose();
        base.Dispose();
    }
}