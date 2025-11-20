using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Radzen;

namespace CamCare.Services
{
    public class IncludedComponentService : DbService<IncludedComponent, IncludedComponentVm>, IIncludedComponentService
    {
        public IncludedComponentService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<IncludedComponentService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

    }

}
