// Interfaccia per parlare col servizio Catalogo, così nei test posso usare Catalog.Api senza farlo partire veramente

using Booking.Api.DTOs;

namespace Booking.Api.Services;

public interface ICatalogClient
{
    Task<SpettacoloDto?> GetSpettacoloAsync(int spettacoloId, CancellationToken ct = default);
    Task<PostoDto?> GetPostoAsync(int postoId, CancellationToken ct = default);
    Task<List<PostoDto>> GetPostiBySalaAsync(int salaId, CancellationToken ct = default);
}