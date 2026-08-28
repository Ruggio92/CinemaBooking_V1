// controller che mostra il dettaglio di un posto

using Catalog.Api.Data;
using Catalog.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/posti")]
[Authorize]
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