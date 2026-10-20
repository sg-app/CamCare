using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<Defective> Defectives { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<IncludedComponent> IncludedComponents { get; set; }
        public DbSet<RepairOrderStatus> RepairOrderStatuses { get; set; }
        public DbSet<RepairOrder> RepairOrders { get; set; }
        public DbSet<RepairPosition> RepairPositions { get; set; }
        public DbSet<LogisticProvider> LogisticProviders { get; set; }
        public DbSet<RepairOrderStatusHistory> RepairOrderStatusHistories { get; set; }
        public DbSet<DataStore> DataStores { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<RepairOrderSerial> RepairOrderSerials { get; set; }
        public DbSet<MailSettings> MailSettings { get; set; }
        public DbSet<MailTemplate> MailTemplates { get; set; }
        public DbSet<RepairOrderMailLog> RepairOrderMailLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RepairOrder>()
                .HasMany(ro => ro.RepairPositions)
                .WithMany(rp => rp.RepairOrders)
                .UsingEntity<RepairOrderRepairPosition>(
                    "RepairOrderRepairPosition",
                    right => right
                        .HasOne(rp => rp.RepairPosition)
                        .WithMany(rp => rp.RepairOrderRepairPositions)
                        .HasForeignKey(rp => rp.RepairPositionId)
                        .OnDelete(DeleteBehavior.Restrict), // <--- Hier das Verhalten für RepairPosition
                    left => left
                        .HasOne(rp => rp.RepairOrder)
                        .WithMany(ro => ro.RepairOrderRepairPositions)
                        .HasForeignKey(rp => rp.RepairOrderId)
                        .OnDelete(DeleteBehavior.Cascade) // Verhalten für RepairOrder
                );

            modelBuilder.Entity<RepairOrder>()
                .HasMany(e => e.Defectives)
                .WithMany(e => e.RepairOrders)
                .UsingEntity(
                    "DefectiveRepairOrder",
                    r => r.HasOne(typeof(Defective)).WithMany().OnDelete(DeleteBehavior.Restrict),
                    l => l.HasOne(typeof(RepairOrder)).WithMany().OnDelete(DeleteBehavior.Cascade)
                );

            modelBuilder.Entity<RepairOrder>()
                .HasMany(e => e.IncludedComponents)
                .WithMany(e => e.RepairOrders)
                .UsingEntity(
                    "IncludedComponentRepairOrder",
                    r => r.HasOne(typeof(IncludedComponent)).WithMany().OnDelete(DeleteBehavior.Restrict),
                    l => l.HasOne(typeof(RepairOrder)).WithMany().OnDelete(DeleteBehavior.Cascade)
                );

            modelBuilder.Entity<RepairOrder>()
               .HasMany(e => e.Employees)
               .WithMany(e => e.RepairOrders)
               .UsingEntity(
                   "EmployeeRepairOrder",
                   r => r.HasOne(typeof(Employee)).WithMany().OnDelete(DeleteBehavior.Restrict),
                   l => l.HasOne(typeof(RepairOrder)).WithMany().OnDelete(DeleteBehavior.Cascade)
               );


            // Die E-Mail-Adressen gehören zum Kunden und können von mehreren Aufträgen
            // verwendet werden. Beim Löschen eines Auftrags werden nur die Zuordnungen
            // entfernt (Cascade); die Adressen selbst bleiben erhalten (Restrict).
            modelBuilder.Entity<RepairOrder>()
               .HasMany(e => e.EmailAddresses)
               .WithMany(e => e.RepairOrders)
               .UsingEntity(
                   "EmailAddressRepairOrder",
                   r => r.HasOne(typeof(EmailAddress)).WithMany().OnDelete(DeleteBehavior.Restrict),
                   l => l.HasOne(typeof(RepairOrder)).WithMany().OnDelete(DeleteBehavior.Cascade)
               );


            modelBuilder.Entity<RepairOrder>()
               .HasOne(ro => ro.RepairOrderStatus)
               .WithMany()
               .HasForeignKey(ro => ro.RepairOrderStatusId)
               .OnDelete(DeleteBehavior.Restrict);

            // Die Lieferanschrift gehört zum Kunden und kann von mehreren Aufträgen
            // verwendet werden. Ein Löschen der Anschrift bei verwendeten Aufträgen
            // wird daher unterbunden (Restrict).
            modelBuilder.Entity<RepairOrder>()
               .HasOne(ro => ro.DeliveryAddress)
               .WithMany(da => da.RepairOrders)
               .HasForeignKey(ro => ro.DeliveryAddressId)
               .OnDelete(DeleteBehavior.Restrict);

            // Die Seriennummern gehören zum Auftrag und werden mit ihm gelöscht.
            modelBuilder.Entity<RepairOrder>()
               .HasMany(ro => ro.RepairOrderSerials)
               .WithOne(s => s.RepairOrder)
               .HasForeignKey(s => s.RepairOrderId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RepairOrderStatus>()
                .HasData(
                    new RepairOrderStatus { Id = 1, Order = 1, Name = "In Anlieferung", Description = "Reparatur wurde von Kunden angemeldet.", BackgroundColor = "rgb(206, 206, 206)", FontColor = "rgb(0, 0, 0)", IsDefault = true },
                    new RepairOrderStatus { Id = 2, Order = 2, Name = "Eingetroffen", Description = "Reparatur ist im Lager eingetroffen.", IsArrived = true },
                    new RepairOrderStatus { Id = 3, Order = 3, Name = "Begutachtung", Description = "Reparatur wird von Mitarbeiter begutachtet." },
                    new RepairOrderStatus { Id = 4, Order = 4, Name = "Begutachtung abgeschlossen", Description = "Begutachtung wurde vom Mitarbeiter abgeschlosen.", BackgroundColor = "rgb(76, 170, 232)", FontColor = "rgb(0, 0, 0)" },
                    new RepairOrderStatus { Id = 5, Order = 5, Name = "Angebot erstellt", Description = "Angebot wurde erstellt." },
                    new RepairOrderStatus { Id = 6, Order = 6, Name = "Reparatur", Description = "Kamera befindet sich in der Reparatur." },
                    new RepairOrderStatus { Id = 7, Order = 7, Name = "Warte auf Ersatzteile", Description = "Reparatur kann nicht fortgesetzt werden da Ersatzteile bestellt wurden." },
                    new RepairOrderStatus { Id = 8, Order = 8, Name = "Reparatur fertig", Description = "Kamera ist fertig repariert." },
                    new RepairOrderStatus { Id = 9, Order = 9, Name = "Versendet", Description = "Kamera wurde versendet.", BackgroundColor = "rgb(125, 218, 88)", FontColor = "rgb(0, 0, 0)" }
                );

            modelBuilder.Entity<LogisticProvider>()
                .HasData(
                    new LogisticProvider { Id = 1, Name = "DHL", IsDefault = true, IsActive = true },
                    new LogisticProvider { Id = 2, Name = "Dachser", IsDefault = false, IsActive = true },
                    new LogisticProvider { Id = 3, Name = "DPD", IsDefault = false, IsActive = true }
                );

            // E-Mail-Vorlagen gehören zu einem Status des Reparaturauftrags: Wird der Status
            // gelöscht, werden seine Vorlagen mit entfernt.
            modelBuilder.Entity<MailTemplate>()
                .HasOne(t => t.RepairOrderStatus)
                .WithMany()
                .HasForeignKey(t => t.RepairOrderStatusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Das Versandprotokoll gehört zum Auftrag und wird mit ihm gelöscht.
            modelBuilder.Entity<RepairOrderMailLog>()
                .HasOne(l => l.RepairOrder)
                .WithMany()
                .HasForeignKey(l => l.RepairOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Wird eine Vorlage gelöscht, bleibt das Versandprotokoll erhalten (SetNull).
            modelBuilder.Entity<RepairOrderMailLog>()
                .HasOne(l => l.MailTemplate)
                .WithMany()
                .HasForeignKey(l => l.MailTemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Ausgangskonfiguration des E-Mail-Versands (genau ein Datensatz).
            modelBuilder.Entity<MailSettings>()
                .HasData(
                    new MailSettings
                    {
                        Id = 1,
                        SmtpHost = string.Empty,
                        SmtpPort = 587,
                        Security = MailSecurityMode.StartTls,
                        TimeoutSeconds = 30,
                        FromAddress = string.Empty,
                        IsEnabled = false,
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                );

            // Beispielvorlagen für die Status "Eingetroffen" (Id 2) und "Versendet" (Id 9).
            // Sie sind zunächst deaktiviert und werden erst nach dem Aktivieren in den
            // Einstellungen automatisch versendet.
            modelBuilder.Entity<MailTemplate>()
                .HasData(
                    new MailTemplate
                    {
                        Id = 1,
                        RepairOrderStatusId = 2,
                        IsEnabled = false,
                        Order = 1,
                        SubjectTemplate = MailSettingsDefaults.ArrivedSubjectTemplate,
                        BodyTemplate = MailSettingsDefaults.ArrivedBodyTemplate,
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new MailTemplate
                    {
                        Id = 2,
                        RepairOrderStatusId = 9,
                        IsEnabled = false,
                        Order = 2,
                        SubjectTemplate = MailSettingsDefaults.ShippedSubjectTemplate,
                        BodyTemplate = MailSettingsDefaults.ShippedBodyTemplate,
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
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
