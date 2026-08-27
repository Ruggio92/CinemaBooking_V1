// DTO per creare una prenotazione

namespace Booking.Api.DTOs;

public record PrenotazioneRequest(int SpettacoloId, string NomeCliente, int? PostoId);

public record PrenotazioneMultiplaRequest(int SpettacoloId, string NomeCliente, List<int> PostiIds);

public record PrenotazioneDto(int Id, int SpettacoloId, string NomeCliente, DateTime DataCreazione, string Stato, List<int> PostiIds);

public record DisponibilitaPostoDto(int PostoId, string Fila, int Numero, bool Occupato);