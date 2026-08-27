// DTO per il posto: quello che torna dalle api e quello che serve per crearne uno nuovo in una sala

namespace Catalog.Api.DTOs;

public record PostoDto(int Id, int SalaId, string Fila, int Numero);

public record CreaPostoRequest(string Fila, int Numero);