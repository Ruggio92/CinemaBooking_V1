// Uno Spettacolo è l'entità "prenotabile", il servizio Prenotazioni farà sempre riferimento a un IDSpettacolo, mai direttamente a un film o a una sala

namespace Catalog.Api.Models;

public class Spettacolo
{
    public int ID { get; set; }
    public string Titolo { get; set; } = string.Empty;

    public int IDSala { get; set; }
    public Sala? Sala { get; set; }

    public DateTime DataOra { get; set; }
}