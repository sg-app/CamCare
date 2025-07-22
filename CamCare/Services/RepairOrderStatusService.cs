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
        // Hier können bei Bedarf spezifische Methoden für RepairOrderStatus ergänzt werden
    }
}
