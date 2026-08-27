// collega un posto a una prenotazione per uno specifico spettacolo

namespace Booking.Api.Models;

public class PostoPrenotato
{
    public int ID { get; set; }

    public int IDPrenotazione { get; set; }
    public Prenotazione? Prenotazione { get; set; }

    public int IDSpettacolo { get; set; }
    public int IDPosto { get; set; }
}