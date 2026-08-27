// una sala del cinema con numero fisso di Posti

namespace Catalog.Api.Models;

public class Sala
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public ICollection<Posto> Posti { get; set; } = new List<Posto>();
}