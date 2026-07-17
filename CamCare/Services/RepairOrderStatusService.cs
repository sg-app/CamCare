using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
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
    }
}
