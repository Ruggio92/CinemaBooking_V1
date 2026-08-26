// Una sala del cinema con numero fisso di Posti

namespace Catalog.Api.Models;

public class Sala
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;

    // Proprietà di navigazione che verrà usata per caricare i posti collegati (1:N), senza generare una nuova colonna nel DB
    public ICollection<Posto> Posti { get; set; } = new List<Posto>();
}