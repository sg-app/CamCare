using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Radzen;

namespace CamCare.Services
{
    public class DefectiveService : DbService<Defective, DefectiveVm>, IDefectiveService
    {
        public DefectiveService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<DefectiveService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

    }

}
