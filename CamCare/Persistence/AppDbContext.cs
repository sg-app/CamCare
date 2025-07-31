using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<CameraType> CameraTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Defective> Defectives { get; set; }
        public DbSet<RepairOrderStatus> RepairOrderStatuses { get; set; }
        public DbSet<RepairOrder> RepairOrders { get; set; }
        public DbSet<RepairPosition> RepairPositions { get; set; }
        public DbSet<LogisticProvider> LogisticProviders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RepairOrder>()
                .HasMany(e => e.RepairPositions)
                .WithMany(e => e.RepairOrders)
                .UsingEntity<RepairOrderRepairPosition>();


            modelBuilder.Entity<RepairOrderStatus>()
                .HasData(
                    new RepairOrderStatus { Id = 1, Order = 1, Name = "In Anlieferung", Description = "Reparatur wurde von Kunden angemeldet.", BackgroundColor = "rgb(206, 206, 206)", FontColor = "rgb(0, 0, 0)" },
                    new RepairOrderStatus { Id = 2, Order = 2, Name = "Eingetroffen", Description = "Reparatur ist im Lager eingetroffen." },
                    new RepairOrderStatus { Id = 3, Order = 3, Name = "Begutachtung", Description = "Reparatur wird von Mitarbeiter begutachtet." },
                    new RepairOrderStatus { Id = 4, Order = 4, Name = "Begutachtung abgeschlossen", Description = "Begutachtung wurde vom Mitarbeiter abgeschlosen.", BackgroundColor = "rgb(76, 170, 232)", FontColor = "rgb(0, 0, 0)" },
                    new RepairOrderStatus { Id = 5, Order = 5, Name = "Angebot erstellt", Description = "Angebot wurde erstellt." },
                    new RepairOrderStatus { Id = 6, Order = 6, Name = "Reparatur", Description = "Kamera befindet sich in der Reparatur." },
                    new RepairOrderStatus { Id = 7, Order = 7, Name = "Warte auf Ersatzteile", Description = "Reparatur kann nicht fortgesetzt werden da Ersatzteile bestellt wurden." },
                    new RepairOrderStatus { Id = 8, Order = 8, Name = "Reparatur fertig", Description = "Kamera ist fertig repariert." },
                    new RepairOrderStatus { Id = 9, Order = 9, Name = "Versendet", Description = "Kamera wurde versendet.", BackgroundColor = "rgb(125, 218, 88)", FontColor = "rgb(0, 0, 0)" }
                );

            modelBuilder.Entity<Defective>()
                .HasData(
                    new Defective { Id = 1, Description = "Display defekt" },
                    new Defective { Id = 2, Description = "Objektiv defekt" },
                    new Defective { Id = 3, Description = "Akku defekt" },
                    new Defective { Id = 4, Description = "Gehäuse defekt" }
                );

            modelBuilder.Entity<RepairPosition>()
                .HasData(
                    new RepairPosition { Id = 1, Description = "Display tauschen" },
                    new RepairPosition { Id = 2, Artikelnummer = "01532", Description = "Objektiv tauschen" },
                    new RepairPosition { Id = 3, Artikelnummer = "0153215", Description = "Akku tauschen" },
                    new RepairPosition { Id = 4, Description = "Gehäuse tauschen" }
                );

            modelBuilder.Entity<CameraType>()
                .HasData(
                    new CameraType { Id = 1, Name = "Mini 3000" },
                    new CameraType { Id = 2, Name = "Mini 3110" },
                    new CameraType { Id = 3, Name = "4540" },
                    new CameraType { Id = 4, Name = "5030" }
                );
            
            modelBuilder.Entity<Camera>()
                .HasData(
                    new Camera { SerialNumber = "1234567890", CameraTypeId = 1, CustomerId = "1" }
                );

            modelBuilder.Entity<Customer>()
                .HasData(
                    new Customer { Id = "1", FirstName = "Max", LastName = "Mustermann", Email = "max@mustermann.de", PhoneNumber = "089190815" }
                    );

            modelBuilder.Entity<Address>()
                .HasData(
                    new Address { Id = 1, CustomerId = "1", AddressType=AddressType.Billing, Street = "Musterstraße 1", PostalCode = "80331", City = "München", Country = "Deutschland" }
                );

            modelBuilder.Entity<LogisticProvider>()
                .HasData(
                    new LogisticProvider { Id = 1, Name = "DHL", IsDefault = true, IsActive = true },
                    new LogisticProvider { Id = 2, Name = "Dachser", IsDefault = false, IsActive = true },
                    new LogisticProvider { Id = 3, Name = "DPD", IsDefault = false, IsActive = true }
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
