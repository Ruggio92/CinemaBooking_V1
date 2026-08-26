namespace Catalog.Api.DTOs;

public record SpettacoloDto(int Id, string Titolo, int SalaId, DateTime DataOra);

public record CreaSpettacoloRequest(string Titolo, int SalaId, DateTime DataOra);
