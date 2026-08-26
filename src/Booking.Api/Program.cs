using Booking.Api.Data;
using Booking.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BookingDb")));

// HttpClient per chiamare Catalog.Api. Uso AddHttpClient invece di crearlo a mano perchè così riusa le connessioni già presenti invece di aprirne sempre di nuove
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