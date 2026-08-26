// Vista di Booking su Catalog: DTO separati apposta dai Model originali, sono due servizi indipendenti

namespace Booking.Api.DTOs;

public record PostoDto(int Id, int SalaId, string Fila, int Numero);

public record SpettacoloDto(int Id, string Titolo, int SalaId, DateTime DataOra);