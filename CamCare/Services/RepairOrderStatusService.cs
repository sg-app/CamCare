using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace CamCare.Services
{
    public class RepairOrderStatusService : DbService<RepairOrderStatus, RepairOrderStatusVm>, IRepairOrderStatusService
    {
        public RepairOrderStatusService(
            IAppDbContextFactory contextFactory,
            IMapper mapper,
            ILogger<RepairOrderStatusService> logger,
            NotificationService notificationService,
            IMasterdataService masterdataService)
            : base(contextFactory, mapper, logger, notificationService, masterdataService)
        {
        }

        public async Task<ServiceResponse<bool>> UpdateOrderAsync(IList<RepairOrderStatusVm> vms)
        {
            try
            {
                var minOrder = vms.Min(x => x.Order);
                for (int i = 0; i < vms.Count; i++)
                {
                    vms[i].Order = minOrder + i;
                }

                using var context = _contextFactory.CreateDbContext();
                var ids = vms.Select(x => x.Id).ToList();
                var entities = context.RepairOrderStatuses.Where(x => ids.Contains(x.Id)).ToList();
                foreach (var vm in vms)
                {
                    var entity = entities.FirstOrDefault(e => e.Id == vm.Id);
                    if (entity != null && entity.Order != vm.Order)
                    {
                        entity.Order = vm.Order;
                    }
                }
                await context.SaveChangesAsync();
                await InvalidateMasterdataCacheAsync();
                return ServiceResponse.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren der Reihenfolge von RepairOrderStatus");
                NotifyError("Fehler beim Aktualisieren der Reihenfolge");
                return ServiceResponse.Failure<bool>("Fehler beim Aktualisieren der Reihenfolge", ex);
            }
        }

        public async Task<ServiceResponse<RepairOrderStatusUsageVm>> GetUsageAsync(int id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var usage = new RepairOrderStatusUsageVm
                {
                    HistoryCount = await context.RepairOrderStatusHistories.CountAsync(h => h.RepairOrderStatusId == id),
                    RepairOrderCount = await context.RepairOrders.CountAsync(o => o.RepairOrderStatusId == id)
                };
                return ServiceResponse.Success(usage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Ermitteln der Verwendung von RepairOrderStatus {StatusId}", id);
                NotifyError("Fehler beim Prüfen der Verwendung des Status");
                return ServiceResponse.Failure<RepairOrderStatusUsageVm>("Fehler beim Prüfen der Verwendung des Status", ex);
            }
        }

        public override async Task<ServiceResponse<bool>> DeleteAsync(object id, bool archive = true)
        {
            // Soft-Delete (Archivieren): Der Status bleibt als Datensatz erhalten,
            // die Fremdschlüssel bleiben gültig -> kein Konflikt, Basis-Implementierung verwenden.
            if (archive)
                return await base.DeleteAsync(id, archive);

            // Hard-Delete: Zugehörige History-Einträge werden NICHT gelöscht, sondern nur ihr
            // Status-FK gelöst – der gespeicherte Statusname/-stil (Snapshot) bleibt erhalten.
            using var context = _contextFactory.CreateDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var statusId = Convert.ToInt32(id);
                var entity = await context.Set<RepairOrderStatus>().FindAsync(statusId);
                if (entity == null)
                    return ServiceResponse.Failure<bool>("Nicht gefunden");

                // Sicherheitscheck: Solange noch Reparaturaufträge diesen Status als aktuellen Status verwenden,
                // kann der Status nicht gelöscht werden (FK Restrict, nicht-nullbare Spalte).
                var referencingOrders = await context.RepairOrders.CountAsync(o => o.RepairOrderStatusId == statusId);
                if (referencingOrders > 0)
                {
                    await transaction.RollbackAsync();
                    var message = $"Der Status kann nicht gelöscht werden, da er noch von {referencingOrders} Reparaturaufträgen als aktueller Status verwendet wird. Bitte weisen Sie diesen Aufträgen zuerst einen anderen Status zu.";
                    NotifyError(message, 7000);
                    return ServiceResponse.Failure<bool>(message);
                }

                // History-FKs lösen – die gespeicherten Statusnamen/Farben (Snapshot) bleiben erhalten.
                var historyEntries = await context.RepairOrderStatusHistories
                    .Where(h => h.RepairOrderStatusId == statusId)
                    .ToListAsync();
                foreach (var historyEntry in historyEntries)
                {
                    historyEntry.RepairOrderStatusId = null;
                }

                context.Set<RepairOrderStatus>().Remove(entity);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                await InvalidateMasterdataCacheAsync();
                return ServiceResponse.Success(true);
            }
            catch (DbUpdateException updateException)
            {
                await transaction.RollbackAsync();
                string errorMessage = "Fehler beim Löschen des Datensatzes";
                if (updateException.InnerException is not null
                    && (updateException.InnerException.Message.Contains("conflicted with the REFERENCE constraint")
                        || updateException.InnerException.Message.Contains("Konflikt mit der REFERENCE-Einschränkung")))
                {
                    errorMessage = "Löschen nicht möglich, da noch verknüpfte Daten existieren.";
                }
                _logger.LogError(updateException, "Fehler in DeleteAsync");
                NotifyError(errorMessage);
                return ServiceResponse.Failure<bool>(errorMessage, updateException);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Fehler in DeleteAsync");
                NotifyError("Fehler beim Löschen des Datensatzes");
                return ServiceResponse.Failure<bool>("Fehler beim Löschen des Datensatzes", ex);
            }
        }
    }
}
