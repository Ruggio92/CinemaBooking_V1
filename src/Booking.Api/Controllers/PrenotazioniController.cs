// Controller che gestisce le prenotazioni: crea, legge il dettaglio e cancella. Il POST è la parte più delicata, gestisce anche il conflitto se due richieste arrivano sullo stesso posto insieme

using Booking.Api.Data;
using Booking.Api.DTOs;
using Booking.Api.Models;
using Booking.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/prenotazioni")]
public class PrenotazioniController : ControllerBase
{
    private readonly BookingDbContext _db;
    private readonly ICatalogClient _catalog;

    public PrenotazioniController(BookingDbContext db, ICatalogClient catalog)
    {
        _db = db;
        _catalog = catalog;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrenotazioneDto>> GetPrenotazione(int id)
    {
        var prenotazione = await _db.Prenotazioni
            .Include(p => p.Posti)
            .FirstOrDefaultAsync(p => p.ID == id);

        if (prenotazione is null)
            return NotFound();

        return Ok(ToDto(prenotazione));
    }

    [HttpPost]
    public async Task<ActionResult<PrenotazioneDto>> CreaPrenotazione(PrenotazioneRequest request)
    {
        var spettacolo = await _catalog.GetSpettacoloAsync(request.SpettacoloId);
        if (spettacolo is null)
            return NotFound($"Spettacolo {request.SpettacoloId} non trovato.");

        int postoId;

        if (request.PostoId.HasValue)
        {
            var posto = await _catalog.GetPostoAsync(request.PostoId.Value);
            if (posto is null)
                return NotFound($"Posto {request.PostoId} non trovato.");

            if (posto.SalaId != spettacolo.SalaId)
                return BadRequest("Il posto non appartiene alla sala di questo spettacolo.");

            postoId = posto.Id;
        }
        else
        {
            var postiSala = await _catalog.GetPostiBySalaAsync(spettacolo.SalaId);
            var occupati = await _db.PostiPrenotati
                .Where(pp => pp.IDSpettacolo == request.SpettacoloId)
                .Select(pp => pp.IDPosto)
                .ToListAsync();

            var postoLibero = postiSala.FirstOrDefault(p => !occupati.Contains(p.Id));
            if (postoLibero is null)
                return Conflict("Nessun posto disponibile per questo spettacolo.");

            postoId = postoLibero.Id;
        }

        var prenotazione = new Prenotazione
        {
            IDSpettacolo = request.SpettacoloId,
            NomeCliente = request.NomeCliente,
            Posti = new List<PostoPrenotato>
            {
                new() { IDSpettacolo = request.SpettacoloId, IDPosto = postoId }
            }
        };

        _db.Prenotazioni.Add(prenotazione);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // posto già prenotato
            return Conflict($"Il posto {postoId} è già stato prenotato per questo spettacolo.");
        }

        return CreatedAtAction(nameof(GetPrenotazione), new { id = prenotazione.ID }, ToDto(prenotazione));
    }

    [HttpPost("multiple")]
    public async Task<ActionResult<PrenotazioneDto>> CreaPrenotazioneMultipla(PrenotazioneMultiplaRequest request)
    {
        if (request.PostiIds is null || request.PostiIds.Count == 0)
            return BadRequest("Specificare almeno un posto.");

        if (request.PostiIds.Distinct().Count() != request.PostiIds.Count)
            return BadRequest("La lista contiene posti duplicati.");

        var spettacolo = await _catalog.GetSpettacoloAsync(request.SpettacoloId);
        if (spettacolo is null)
            return NotFound($"Spettacolo {request.SpettacoloId} non trovato.");

        var postiSala = await _catalog.GetPostiBySalaAsync(spettacolo.SalaId);
        var postiSalaIds = postiSala.Select(p => p.Id).ToHashSet();

        var postiNonValidi = request.PostiIds.Where(id => !postiSalaIds.Contains(id)).ToList();
        if (postiNonValidi.Count > 0)
            return NotFound($"I posti {string.Join(", ", postiNonValidi)} non esistono nella sala di questo spettacolo.");

        // dice quale posto è occupato
        var occupati = await _db.PostiPrenotati
            .Where(pp => pp.IDSpettacolo == request.SpettacoloId && request.PostiIds.Contains(pp.IDPosto))
            .Select(pp => pp.IDPosto)
            .ToListAsync();

        if (occupati.Count > 0)
            return Conflict($"I posti {string.Join(", ", occupati)} sono già stati prenotati per questo spettacolo.");

        var prenotazione = new Prenotazione
        {
            IDSpettacolo = request.SpettacoloId,
            NomeCliente = request.NomeCliente,
            Posti = request.PostiIds
                .Select(postoId => new PostoPrenotato { IDSpettacolo = request.SpettacoloId, IDPosto = postoId })
                .ToList()
        };

        _db.Prenotazioni.Add(prenotazione);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // caso in cui qualcuno ha prenotato un posto mentre facevo la inser
            return Conflict("Uno o più posti richiesti sono stati appena prenotati da un'altra richiesta. Riprova.");
        }

        return CreatedAtAction(nameof(GetPrenotazione), new { id = prenotazione.ID }, ToDto(prenotazione));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> CancellaPrenotazione(int id)
    {
        var prenotazione = await _db.Prenotazioni
            .Include(p => p.Posti)
            .FirstOrDefaultAsync(p => p.ID == id);

        if (prenotazione is null)
            return NotFound();

        if (prenotazione.Stato == StatoPrenotazione.Cancellata)
            return BadRequest("Prenotazione già cancellata.");

        _db.PostiPrenotati.RemoveRange(prenotazione.Posti);
        prenotazione.Stato = StatoPrenotazione.Cancellata;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static PrenotazioneDto ToDto(Prenotazione p) => new(
        p.ID,
        p.IDSpettacolo,
        p.NomeCliente,
        p.DataCreazione,
        p.Stato.ToString(),
        p.Posti.Select(ps => ps.IDPosto).ToList());
}