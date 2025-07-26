using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CamCare.Domain;
using CamCare.Models;
using CamCare.Interfaces.Services;
using CamCare.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using Radzen;

namespace CamCare.Services
{
    public class RepairOrderStatusService : DbService<RepairOrderStatus, RepairOrderStatusVm>, IRepairOrderStatusService
    {
        public RepairOrderStatusService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<RepairOrderStatusService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

        public async Task<ServiceResponse<bool>> UpdateOrderAsync(IList<RepairOrderStatusVm> vms)
        {
            try
            {
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
