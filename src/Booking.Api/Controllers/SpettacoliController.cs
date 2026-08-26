// Controller che dice quali posti sono liberi/occupati per uno spettacolo, chiamato prima di far scegliere il posto al cliente come controllo

using Booking.Api.Data;
using Booking.Api.DTOs;
using Booking.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/spettacoli")]
public class SpettacoliController : ControllerBase
{
    private readonly BookingDbContext _db;
    private readonly ICatalogClient _catalog;

    public SpettacoliController(BookingDbContext db, ICatalogClient catalog)
    {
        _db = db;
        _catalog = catalog;
    }

    [HttpGet("{spettacoloId:int}/disponibilita")]
    public async Task<ActionResult<List<DisponibilitaPostoDto>>> GetDisponibilita(int spettacoloId)
    {
        var spettacolo = await _catalog.GetSpettacoloAsync(spettacoloId);
        if (spettacolo is null)
            return NotFound($"Spettacolo {spettacoloId} non trovato.");

        var postiSala = await _catalog.GetPostiBySalaAsync(spettacolo.SalaId);

        var occupati = await _db.PostiPrenotati
            .Where(pp => pp.IDSpettacolo == spettacoloId)
            .Select(pp => pp.IDPosto)
            .ToListAsync();

        var risultato = postiSala
            .Select(p => new DisponibilitaPostoDto(p.Id, p.Fila, p.Numero, occupati.Contains(p.Id)))
            .ToList();

        return Ok(risultato);
    }
}