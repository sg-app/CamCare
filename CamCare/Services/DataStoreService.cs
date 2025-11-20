using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;

namespace CamCare.Services
{
    public class DataStoreService : DbService<DataStore, DataStoreVm>, IDataStoreService
    {
        public DataStoreService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<DataStoreService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        { }


        public async Task<ServiceResponse<List<DataStoreVm>>> GetFromRepairOrderAsync(int repairOrderId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var items = await context.DataStores
                    .AsNoTracking()
                    .Where(ds => ds.RepairOrderId == repairOrderId)
                    .ToListAsync();

                var vms = items.Select(_mapper.Map<DataStore, DataStoreVm>).ToList();

                return ServiceResponse.Success(vms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetFromRepairOrderAsync {id}", repairOrderId);
                NotifyError("Fehler beim Abrufen der Daten");
                return ServiceResponse.Failure<List<DataStoreVm>>("Fehler beim Abrufen der Daten", ex);
            }
        }
    }
}

