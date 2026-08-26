// Program.cs di Booking.Api: configura DB, HttpClient verso Catalog, Swagger e applica le migration all'avvio

using Booking.Api.Data;
using Booking.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BookingDb"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
{
    var baseUrl = builder.Configuration["CatalogApi:BaseUrl"]
        ?? throw new InvalidOperationException("CatalogApi:BaseUrl non configurato");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Booking API",
        Version = "v1",
        Description = "Gestisce prenotazioni, disponibilità e cancellazioni."
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }