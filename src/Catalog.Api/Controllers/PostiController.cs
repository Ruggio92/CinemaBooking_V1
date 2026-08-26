// Controller che mostra il dettaglio di un posto, chiamato anche da Booking.Api per validare una prenotazione

using Catalog.Api.Data;
using Catalog.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/posti")]
public class PostiController : ControllerBase
{
    private readonly CatalogDbContext _db;

    public PostiController(CatalogDbContext db)
    {
        _db = db;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostoDto>> GetPosto(int id)
    {
        var posto = await _db.Posti.FindAsync(id);
        if (posto is null)
            return NotFound();

        return Ok(new PostoDto(posto.ID, posto.IDSala, posto.Fila, posto.Numero));
    }
}