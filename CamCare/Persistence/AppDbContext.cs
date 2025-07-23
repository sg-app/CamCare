using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<RepairOrderStatus> RepairOrderStatuses { get; set; }
        public DbSet<RepairOrder> RepairOrders { get; set; }
        public DbSet<RepairPosition> RepairPositions { get; set; }
        public DbSet<RepairOrderRepairPosition> RepairOrderRepairPositions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the many-to-many relationship between RepairOrder and RepairPosition
            modelBuilder.Entity<RepairOrder>()
                .HasMany(e=>e.RepairPositions)
                .WithMany(e=> e.RepairOrders)
                .UsingEntity<RepairOrderRepairPosition>();

            modelBuilder.Entity<RepairOrderStatus>()
                .HasData(
                    new RepairOrderStatus { Id = 1, Order = 1, Name = "In Anlieferung", Description="Reparatur wurde von Kunden angemeldet." , BackgroundColor = "rgb(206, 206, 206)", FontColor = "rgb(0, 0, 0)" },
                    new RepairOrderStatus { Id = 2, Order = 2, Name = "Eingetroffen", Description="Reparatur ist im Lager eingetroffen." },
                    new RepairOrderStatus { Id = 3, Order = 3, Name = "Begutachtung", Description="Reparatur wird von Mitarbeiter begutachtet." },
                    new RepairOrderStatus { Id = 4, Order = 4, Name = "Begutachtung abgeschlossen", Description = "Begutachtung wurde vom Mitarbeiter abgeschlosen.", BackgroundColor = "rgb(76, 170, 232)", FontColor = "rgb(0, 0, 0)" },
                    new RepairOrderStatus { Id = 5, Order = 5, Name = "Angebot erstellt", Description = "Angebot wurde erstellt." },
                    new RepairOrderStatus { Id = 6, Order = 6, Name = "Reparatur", Description = "Kamera befindet sich in der Reparatur." },
                    new RepairOrderStatus { Id = 7, Order = 7, Name = "Warte auf Ersatzteile", Description = "Reparatur kann nicht fortgesetzt werden da Ersatzteile bestellt wurden." },
                    new RepairOrderStatus { Id = 8, Order = 8, Name = "Reparatur fertig", Description = "Kamera ist fertig repariert." },
                    new RepairOrderStatus { Id = 9, Order = 9, Name = "Versendet", Description = "Kamera wurde versendet.", BackgroundColor = "rgb(125, 218, 88)", FontColor = "rgb(0, 0, 0)" }
                );

            modelBuilder.Entity<AddressType>()
                .HasData(
                    new AddressType { Id = 1, Name = "Rechnungsadresse" },
                    new AddressType { Id = 2, Name = "Lieferadresse" }
                );
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                    entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
