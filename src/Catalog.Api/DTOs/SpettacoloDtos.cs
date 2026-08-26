// DTO per lo spettacolo: quello che torna dalle API e quello che serve per crearne uno nuovo

namespace Catalog.Api.DTOs;

public record SpettacoloDto(int Id, string Titolo, int SalaId, DateTime DataOra);

public record CreaSpettacoloRequest(string Titolo, int SalaId, DateTime DataOra);