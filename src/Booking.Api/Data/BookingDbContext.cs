// DbContext di Booking.Api, mappa Prenotazione/PostoPrenotato

using Booking.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }
    public DbSet<Prenotazione> Prenotazioni => Set<Prenotazione>();
    public DbSet<PostoPrenotato> PostiPrenotati => Set<PostoPrenotato>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostoPrenotato>()
            .HasOne(pp => pp.Prenotazione)
            .WithMany(p => p.Posti)
            .HasForeignKey(pp => pp.IDPrenotazione);

        modelBuilder.Entity<PostoPrenotato>()
            .HasIndex(pp => new { pp.IDSpettacolo, pp.IDPosto })
            .IsUnique();
    }
}