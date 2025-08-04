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

        public override async Task<ServiceResponse<CustomerVm>> CreateAsync(CustomerVm vm)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var entity = new Customer();
                _mapper.Map(vm, entity);
                entity.CreatedAt = DateTime.UtcNow;

                // Addresses anlegen
                entity.Addresses = new List<Address>();
                if (vm.Addresses != null)
                {
                    foreach (var addrVm in vm.Addresses)
                    {
                        var address = _mapper.Map<AddressVm, Address>(addrVm);
                        address.CreatedAt = DateTime.UtcNow;
                        address.CustomerId = entity.Id;
                        entity.Addresses.Add(address);
                    }
                }

                context.Customers.Add(entity);
                await context.SaveChangesAsync();

                var resultVm = _mapper.Map<Customer, CustomerVm>(entity);
                return ServiceResponse.Success(resultVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in CreateAsync");
                NotifyError("Fehler beim Erstellen des Kunden");
                return ServiceResponse.Failure<CustomerVm>("Fehler beim Erstellen des Kunden", ex);
            }
        }

        public override async Task<ServiceResponse<CustomerVm>> UpdateAsync(object id, CustomerVm vm)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var entity = await context.Customers
                    .Include(c => c.Addresses)
                    .FirstOrDefaultAsync(c => c.Id == (string)id);

                if (entity == null)
                    return ServiceResponse.Failure<CustomerVm>("Nicht gefunden");

                _mapper.Map(vm, entity);
                entity.UpdatedAt = DateTime.UtcNow;

                // --- Addresses synchronisieren ---
                var vmAddresses = vm.Addresses ?? new List<AddressVm>();

                // Entfernte Adressen löschen
                var toRemove = entity.Addresses
                    .Where(a => !vmAddresses.Any(vmA => vmA.Id == a.Id))
                    .ToList();
                foreach (var addr in toRemove)
                    context.Addresses.Remove(addr);

                // Hinzufügen/Aktualisieren
                foreach (var addrVm in vmAddresses)
                {
                    if (addrVm.Id == 0)
                    {
                        // Neue Adresse
                        var newAddress = _mapper.Map<AddressVm, Address>(addrVm);
                        newAddress.CustomerId = entity.Id;
                        newAddress.CreatedAt = DateTime.UtcNow;
                        entity.Addresses.Add(newAddress);
                    }
                    else
                    {
                        // Existierende Adresse aktualisieren
                        var existing = entity.Addresses.FirstOrDefault(a => a.Id == addrVm.Id);
                        if (existing != null)
                        {
                            _mapper.Map(addrVm, existing);
                            existing.UpdatedAt = DateTime.UtcNow;
                        }
                    }
                }

                await context.SaveChangesAsync();

                var resultVm = _mapper.Map<Customer, CustomerVm>(entity);
                return ServiceResponse.Success(resultVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in UpdateAsync");
                NotifyError("Fehler beim Aktualisieren des Kunden");
                return ServiceResponse.Failure<CustomerVm>("Fehler beim Aktualisieren des Kunden", ex);
            }
        }
    }
}
