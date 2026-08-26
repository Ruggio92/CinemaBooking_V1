// Collega un posto a una prenotazione, per uno specifico spettacolo. IDPosto NON è una foreign key verso un database, ma un ID "copiato" dal servizio Catalogo
// La sua validità viene verificata via HTTP al momento della creazione, non dal database

namespace Booking.Api.Models;

public class PostoPrenotato
{
    public int ID { get; set; }

    public int IDPrenotazione { get; set; }
    public Prenotazione? Prenotazione { get; set; }

    public int IDSpettacolo { get; set; }
    public int IDPosto { get; set; }
}