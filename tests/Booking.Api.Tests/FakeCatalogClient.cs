using Booking.Api.DTOs;
using Booking.Api.Services;

namespace Booking.Api.Tests;

public class FakeCatalogClient : ICatalogClient
{
    public Dictionary<int, SpettacoloDto> Spettacoli { get; } = new();
    public Dictionary<int, PostoDto> Posti { get; } = new();

    public Task<SpettacoloDto?> GetSpettacoloAsync(int id, CancellationToken ct = default) => Task.FromResult(Spettacoli.GetValueOrDefault(id));

    public Task<PostoDto?> GetPostoAsync(int id, CancellationToken ct = default) => Task.FromResult(Posti.GetValueOrDefault(id));

    public Task<List<PostoDto>> GetPostiBySalaAsync(int salaId, CancellationToken ct = default) => Task.FromResult(Posti.Values.Where(p => p.SalaId == salaId).ToList());
}