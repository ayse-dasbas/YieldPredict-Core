using Microsoft.EntityFrameworkCore;
using YieldPredict.Core.Entities;
using YieldPredict.Infrastructure.Persistence.Configurations;

namespace YieldPredict.Infrastructure.Persistence;

// Infrastructure katmanındaki EF Core DbContext; domain entity'lerini ilişkisel modele eşleyerek Core katmanı ile veritabanı arasında anti-corruption boundary oluşturur.
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<HotelBooking> HotelBookings { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Kalıcılık kuralları ayrı konfigurasyon sınıflarında merkezileştirilerek, Clean Architecture prensiplerine uygun şekilde domain nesneleri veritabanı ayrıntılarından izole edilir.
        modelBuilder.ApplyConfiguration(new HotelBookingConfiguration());
    }
}
