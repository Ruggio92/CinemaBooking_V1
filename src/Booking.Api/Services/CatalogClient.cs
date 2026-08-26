// Implementazione ICatalogClient: fa le chiamate HTTP reali verso Catalog.Api

using Booking.Api.DTOs;

namespace Booking.Api.Services;

public class CatalogClient : ICatalogClient
{
    private readonly HttpClient _httpClient;

    public CatalogClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SpettacoloDto?> GetSpettacoloAsync(int spettacoloId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/spettacoli/{spettacoloId}", ct);

        // 404 è una risposta legittima (lo spettacolo non esiste), la tratto come "non trovato"
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SpettacoloDto>(cancellationToken: ct);
    }

    public async Task<PostoDto?> GetPostoAsync(int postoId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/posti/{postoId}", ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PostoDto>(cancellationToken: ct);
    }

    public async Task<List<PostoDto>> GetPostiBySalaAsync(int salaId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/sale/{salaId}/posti", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<PostoDto>>(cancellationToken: ct) ?? new List<PostoDto>();
    }
}