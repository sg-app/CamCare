using CamCare.Domain;
using CamCare.Extensions;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace CamCare.Services
{
    public class CameraService : DbService<Camera, CameraVm>, ICameraService
    {
        public CameraService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<CameraService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

        public override async Task<ServiceResponse<Paginated<CameraVm>>> GetAllAsync(LoadDataArgs args, Expression<Func<Camera, bool>>? predicate = null)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var (totalCount, query) = context.Cameras
                    .AsNoTracking()
                    .Include(x => x.CameraType)
                    .AsQueryable()
                    .LoadByLoadDataArgs(args, predicate);

                var items = await query.ToListAsync();
                var vms = items.Select(e => _mapper.Map<Camera, CameraVm>(e)).ToList();

                var paginated = new Paginated<CameraVm>
                {
                    Items = vms,
                    TotalCount = totalCount
                };
                return ServiceResponse.Success(paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetAllAsync(LoadDataArgs)");
                NotifyError("Fehler beim Abrufen der Daten");
                return ServiceResponse.Failure<Paginated<CameraVm>>("Fehler beim Abrufen der Daten", ex);
            }
        }
    }

}
