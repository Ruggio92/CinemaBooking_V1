// Un posto fisico all'interno di una sala. Esiste indipendentemente dagli spettacoli: la poltrona "A12" esiste per ogni spettacolo proiettato in quella sala

namespace Catalog.Api.Models;

public class Posto
{
    public int ID { get; set; }

    public int IDSala { get; set; }
    public Sala? Sala { get; set; }

    public string Fila { get; set; } = string.Empty;    // es. "A", "B", "C"...
    public int Numero { get; set; }                     // es. "12", "13", "14"...
}