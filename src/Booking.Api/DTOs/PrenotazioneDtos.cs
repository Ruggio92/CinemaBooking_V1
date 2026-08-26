// DTO per creare una prenotazione e per le risposte dei controller (prenotazione e disponibilità posti)

namespace Booking.Api.DTOs;

public record CreaPrenotazioneRequest(int SpettacoloId, string NomeCliente, int? PostoId);

public record PrenotazioneDto(int Id, int SpettacoloId, string NomeCliente, DateTime DataCreazione, string Stato, List<int> PostiIds);

public record DisponibilitaPostoDto(int PostoId, string Fila, int Numero, bool Occupato);