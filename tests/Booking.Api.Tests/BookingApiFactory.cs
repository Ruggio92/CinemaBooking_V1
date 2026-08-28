using Booking.Api.Data;
using Booking.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Booking.Api.Tests;

public class BookingApiFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");
    public FakeCatalogClient CatalogClient { get; } = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "Chiave-Di-Test-Solo-Per-Xunit-Non-Usare-In-Produzione",
                ["Jwt:Issuer"] = "CinemaBooking",
                ["Jwt:Audience"] = "CinemaBookingApi",
                ["CatalogApi:BaseUrl"] = "http://localhost"
            });
        });

        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BookingDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<BookingDbContext>>();

            _connection.Open();
            services.AddDbContext<BookingDbContext>(options => options.UseSqlite(_connection).ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.RemoveAll<ICatalogClient>();
            services.AddSingleton<ICatalogClient>(CatalogClient);

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            db.Database.EnsureCreated();

        });
    }

    public new void Dispose()
    {
        _connection.Dispose();
        base.Dispose();
    }
}