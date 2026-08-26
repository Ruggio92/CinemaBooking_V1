using Catalog.Api.Data;
using Catalog.Api.DTOs;
using Catalog.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/spettacoli")]
public class SpettacoliController : ControllerBase
{
    private readonly CatalogDbContext _db;

    public SpettacoliController(CatalogDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<SpettacoloDto>>> GetSpettacoli()
    {
        var spettacoli = await _db.Spettacoli
            .Select(s => new SpettacoloDto(s.ID, s.Titolo, s.IDSala, s.DataOra))
            .ToListAsync();

        return Ok(spettacoli);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpettacoloDto>> GetSpettacolo(int id)
    {
        var spettacolo = await _db.Spettacoli.FindAsync(id);
        if (spettacolo is null)
            return NotFound();

        return Ok(new SpettacoloDto(spettacolo.ID, spettacolo.Titolo, spettacolo.IDSala, spettacolo.DataOra));
    }

    [HttpPost]
    public async Task<ActionResult<SpettacoloDto>> CreaSpettacolo(CreaSpettacoloRequest request)
    {
        var salaEsiste = await _db.Sale.AnyAsync(s => s.ID == request.SalaId);
        if (!salaEsiste)
            return BadRequest($"La sala {request.SalaId} non esiste.");

        var spettacolo = new Spettacolo
        {
            Titolo = request.Titolo,
            IDSala = request.SalaId,
            DataOra = request.DataOra
        };

        _db.Spettacoli.Add(spettacolo);
        await _db.SaveChangesAsync();

        var dto = new SpettacoloDto(spettacolo.ID, spettacolo.Titolo, spettacolo.IDSala, spettacolo.DataOra);
        return CreatedAtAction(nameof(GetSpettacolo), new { id = spettacolo.ID }, dto);
    }
}