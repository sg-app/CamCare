using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;

namespace CamCare.Services
{
    public class LogisticProviderService : DbService<LogisticProvider, LogisticProviderVm>, ILogisticProviderService
    {
        public LogisticProviderService(
            IAppDbContextFactory contextFactory,
            IMapper mapper,
            ILogger<LogisticProviderService> logger,
            NotificationService notificationService,
            IMasterdataService masterdataService)
            : base(contextFactory, mapper, logger, notificationService, masterdataService)
        {
        }

        public override async Task<ServiceResponse<List<LogisticProviderVm>>> GetAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            var entities = await context.LogisticProviders
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync();

            var viewModels = entities.Select(_mapper.Map<LogisticProvider, LogisticProviderVm>).ToList();

            return ServiceResponse.Success(viewModels);
        }
    }

}
