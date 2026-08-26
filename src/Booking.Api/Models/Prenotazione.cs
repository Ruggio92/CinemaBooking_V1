// Una prenotazione: un cliente che prenota uno o più posti per uno spettacolo. Una singola Prenotazione può contenere più PostiPrenotati ma resta legata a un solo spettacolo
// Per prenotare più spettacoli insieme, il client fa più chiamate (una Prenotazione per spettacolo)

namespace Booking.Api.Models;

public enum StatoPrenotazione
{
    Attiva = 0,
    Cancellata = 1
}

public class Prenotazione
{
    public int ID { get; set; }

    public int IDSpettacolo { get; set; }

    public string NomeCliente { get; set; } = string.Empty;

    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    public StatoPrenotazione Stato { get; set; } = StatoPrenotazione.Attiva;

    public ICollection<PostoPrenotato> Posti { get; set; } = new List<PostoPrenotato>();
}