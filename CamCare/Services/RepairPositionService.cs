using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Radzen;

namespace CamCare.Services
{
    public class RepairPositionService : DbService<RepairPosition, RepairPositionVm>, IRepairPositionService
    {
        public RepairPositionService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<RepairPositionService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

    }

}
