namespace Catalog.Api.DTOs;

// DTO = quello che esce/entra dalle API. Non uso le entità EF Core (Sala,
// Posto, Spettacolo) direttamente nei controller: se un domani aggiungo un
// campo interno all'entità (es. una colonna tecnica), non voglio che finisca
// per forza nella risposta JSON senza che sia una scelta esplicita.

public record SalaDto(int Id, string Nome);

public record CreaSalaRequest(string Nome);
