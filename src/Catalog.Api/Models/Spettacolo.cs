// uno Spettacolo è l'entità prenotabile non la sala

namespace Catalog.Api.Models;

public class Spettacolo
{
    public int ID { get; set; }
    public string Titolo { get; set; } = string.Empty;

    public int IDSala { get; set; }
    public Sala? Sala { get; set; }

    public DateTime DataOra { get; set; }
}