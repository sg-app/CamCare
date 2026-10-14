using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Radzen;

namespace CamCare.Services
{
    public class WorkService : DbService<Work, WorkVm>, IWorkService
    {
        public WorkService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<WorkService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

    }

}
