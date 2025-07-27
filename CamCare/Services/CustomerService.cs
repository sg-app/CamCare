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
    public class CustomerService : DbService<Customer, CustomerVm>, ICustomerService
    {
        public CustomerService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<CustomerService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

        public override async Task<ServiceResponse<Paginated<CustomerVm>>> GetAllAsync(LoadDataArgs args, Expression<Func<Customer, bool>>? predicate = null)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var (totalCount, query) = context.Customers
                    .AsNoTracking()
                    .Include(x => x.Addresses)
                    .Include(x => x.Cameras)
                        .ThenInclude(c => c.CameraType)
                    .AsQueryable()
                    .LoadByLoadDataArgs(args, predicate);


                var items = await query.ToListAsync();
                var vms = items.Select(e => _mapper.Map<Customer, CustomerVm>(e)).ToList();

                var paginated = new Paginated<CustomerVm>
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
                return ServiceResponse.Failure<Paginated<CustomerVm>>("Fehler beim Abrufen der Daten", ex);
            }
        }
    }
}
