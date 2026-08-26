// Ponte tra le varie entità (Sala, Posto, Spettacolo) e le tabelle SQL. Ogni DbSet diventa una tabella e lo schema si genera con le migration EF Core

using Catalog.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Sala> Sale => Set<Sala>();
    public DbSet<Posto> Posti => Set<Posto>();
    public DbSet<Spettacolo> Spettacoli => Set<Spettacolo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Un posto (Fila + Numero) deve essere unico all'interno della stessa sala, non possono esistere due poltrone "A12" nella stessa Sala
        modelBuilder.Entity<Posto>()
            .HasIndex(p => new { p.IDSala, p.Fila, p.Numero })
            .IsUnique();

        modelBuilder.Entity<Posto>()
            .HasOne(p => p.Sala)
            .WithMany(s => s.Posti)
            .HasForeignKey(p => p.IDSala);

        modelBuilder.Entity<Spettacolo>()
            .HasOne(s => s.Sala)
            .WithMany()
            .HasForeignKey(s => s.IDSala);
    }
}