namespace Catalog.Api.DTOs;

public record PostoDto(int Id, int SalaId, string Fila, int Numero);

public record CreaPostoRequest(string Fila, int Numero);
