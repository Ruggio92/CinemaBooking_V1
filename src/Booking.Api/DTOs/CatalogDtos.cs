// vista di Booking su Catalog: DTO separati dai model originali perchè sono due servizi indipendetni

namespace Booking.Api.DTOs;

public record PostoDto(int Id, int SalaId, string Fila, int Numero);

public record SpettacoloDto(int Id, string Titolo, int SalaId, DateTime DataOra);