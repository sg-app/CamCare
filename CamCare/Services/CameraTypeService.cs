using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Radzen;

namespace CamCare.Services
{
    public class CameraTypeService : DbService<CameraType, CameraTypeVm>, ICameraTypeService
    {
        public CameraTypeService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<CameraTypeService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }
    }
}
