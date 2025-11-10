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
        private readonly IAmicronDataService _amicronData;

        public CameraService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<CameraService> logger, NotificationService notificationService, IAmicronDataService amicronData)
            : base(contextFactory, mapper, logger, notificationService)
        {
            _amicronData = amicronData;
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
                foreach( var vm in vms)
                {
                    var response = await _amicronData.GetAddressByCustomerIdAsync(vm.CustomerId);
                    if (response.Success && response.Data is not null)
                        vm.Customer = response.Data;
                }

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

        public async Task<ServiceResponse<Paginated<CameraVm>>> GetForDropdownAsync(LoadDataArgs args, int customerId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var (totalCount, query) = context.Cameras
                    .AsNoTracking()
                    .Include(x => x.CameraType)
                    .AsQueryable()
                    .LoadByLoadDataArgs(args);

                var items = await query.ToListAsync();
                var vmsFromCamCareDatabase = items.Select(_mapper.Map<Camera, CameraVm>).ToList();
                

                var response = await _amicronData.GetSerialsFromCustomerIdAsync(args, customerId);
                var vmsFromAmicronData = response.Data?.Items.Select(e => new CameraVm
                {
                    CustomerId = e.KundenLfdNr ?? 0,
                    SerialNumber = e.Seriennummer ?? string.Empty,
                   
                }).ToList();
                
                
                var combinedVms = vmsFromCamCareDatabase.Concat(vmsFromAmicronData ?? Enumerable.Empty<CameraVm>()).ToList();

                var paginated = new Paginated<CameraVm>
                {
                    Items = combinedVms,
                    TotalCount = totalCount + (response.Data?.TotalCount ?? 0)
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
