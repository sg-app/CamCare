using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Radzen;

namespace CamCare.Services
{
    public class EmployeeService : DbService<Employee, EmployeeVm>, IEmployeeService
    {
        public EmployeeService(
            IAppDbContextFactory contextFactory,
            IMapper mapper,
            ILogger<EmployeeService> logger,
            NotificationService notificationService,
            IMasterdataService masterdataService)
            : base(contextFactory, mapper, logger, notificationService, masterdataService)
        {
        }
    }
}
