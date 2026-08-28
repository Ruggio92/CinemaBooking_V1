// controller che gestisce le sale

using Catalog.Api.Data;
using Catalog.Api.DTOs;
using Catalog.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/sale")]
[Authorize]
public class SaleController : ControllerBase
{
    private readonly CatalogDbContext _db;

    public SaleController(CatalogDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<SalaDto>>> GetSale()
    {
        var sale = await _db.Sale
            .Select(s => new SalaDto(s.ID, s.Nome))
            .ToListAsync();

        return Ok(sale);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SalaDto>> GetSala(int id)
    {
        var sala = await _db.Sale.FindAsync(id);
        if (sala is null)
            return NotFound();

        return Ok(new SalaDto(sala.ID, sala.Nome));
    }

    [HttpPost]
    public async Task<ActionResult<SalaDto>> CreaSala(CreaSalaRequest request)
    {
        var sala = new Sala { Nome = request.Nome };
        _db.Sale.Add(sala);
        await _db.SaveChangesAsync();

        var dto = new SalaDto(sala.ID, sala.Nome);
        return CreatedAtAction(nameof(GetSala), new { id = sala.ID }, dto);
    }

    [HttpGet("{salaId:int}/posti")]
    public async Task<ActionResult<List<PostoDto>>> GetPostiDiSala(int salaId)
    {
        var salaEsiste = await _db.Sale.AnyAsync(s => s.ID == salaId);
        if (!salaEsiste)
            return NotFound($"Sala {salaId} non trovata.");

        var posti = await _db.Posti
            .Where(p => p.IDSala == salaId)
            .Select(p => new PostoDto(p.ID, p.IDSala, p.Fila, p.Numero))
            .ToListAsync();

        return Ok(posti);
    }

    [HttpPost("{salaId:int}/posti")]
    public async Task<ActionResult<PostoDto>> CreaPosto(int salaId, CreaPostoRequest request)
    {
        var salaEsiste = await _db.Sale.AnyAsync(s => s.ID == salaId);
        if (!salaEsiste)
            return NotFound($"Sala {salaId} non trovata.");

        var posto = new Posto { IDSala = salaId, Fila = request.Fila, Numero = request.Numero };
        _db.Posti.Add(posto);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // il posto è già occupato per questa sala
            return Conflict($"Il posto {request.Fila}{request.Numero} esiste già nella sala {salaId}.");
        }

        var dto = new PostoDto(posto.ID, posto.IDSala, posto.Fila, posto.Numero);
        return CreatedAtAction(
            nameof(PostiController.GetPosto),
            "Posti",
            new { id = posto.ID },
            dto);
    }
}