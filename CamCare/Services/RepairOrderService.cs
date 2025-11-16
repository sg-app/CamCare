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
    public class RepairOrderService : DbService<RepairOrder, RepairOrderVm>, IRepairOrderService
    {
        private readonly IAmicronDataService _amicronDataService;

        public RepairOrderService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<RepairOrderService> logger, NotificationService notificationService, IAmicronDataService amicronDataService)
            : base(contextFactory, mapper, logger, notificationService)
        {
            _amicronDataService = amicronDataService;
        }

        public async Task<ServiceResponse<Paginated<RepairOrderVm>>> GetAllAsync(LoadDataArgs args, bool viewCurrentOrders)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.RepairOrders
                .AsNoTracking();
            if (args.Filter != null)
            {
                query = query.Where(args.Filter);
            }
            if (args.OrderBy != null)
            {
                query = query.OrderBy(args.OrderBy);
            }
            var totalCount = query.Count();
            if (args.Skip.HasValue)
            {
                query = query.Skip(args.Skip.Value);
            }
            if (args.Top.HasValue)
            {
                query = query.Take(args.Top.Value);
            }
            query = query
                .Include(i => i.RepairOrderStatus)
                .Include(i => i.RepairOrderStatusHistory)
                    .ThenInclude(i => i.RepairOrderStatus)
                .Include(i => i.LogisticProvider)
                .Include(i => i.Employees)
                .Include(i => i.Defectives)
                .Include(i => i.IncludedComponents)
                .Include(i => i.RepairOrderRepairPositions)
                    .ThenInclude(rp => rp.RepairPosition);

            if (viewCurrentOrders)
                query = query.Where(f => !f.RepairOrderStatus.IsOrderClose);

            var items = await query.ToListAsync();

            var vms = items.Select(_mapper.Map<RepairOrder, RepairOrderVm>).ToList();

            foreach (var vm in vms)
            {
                if (!vm.CustomerId.HasValue)
                    continue;

                var customerResponse = await _amicronDataService.GetAddressByCustomerIdAsync(vm.CustomerId.Value);
                if (!customerResponse.Success)
                    continue;

                vm.Customer = customerResponse.Data;
            }

            var paginated = new Paginated<RepairOrderVm>
            {
                Items = vms,
                TotalCount = totalCount
            };
            return ServiceResponse.Success(paginated);
        }

        public override async Task<ServiceResponse<RepairOrderVm>> CreateAsync(RepairOrderVm vm)
        {
            using var context = _contextFactory.CreateDbContext();

            var entity = new RepairOrder();
            _mapper.Map(vm, entity);

            // Defectives zuordnen oder anlegen
            entity.Defectives = new List<Defective>();
            if (vm.Defectives != null)
            {
                foreach (var defVm in vm.Defectives)
                {
                    var defective = await context.Defectives.FirstOrDefaultAsync(d => d.Description == defVm.Description);
                    if (defective == null)
                    {
                        defective = new Defective { Description = defVm.Description, CreatedAt = DateTime.UtcNow };
                        context.Defectives.Add(defective);
                        await context.SaveChangesAsync();
                    }
                    entity.Defectives.Add(defective);
                }
            }

            // Included Components zuordnen oder anlegen
            entity.IncludedComponents = new List<IncludedComponent>();
            if (vm.IncludedComponents != null)
            {
                foreach (var incVm in vm.IncludedComponents)
                {
                    var includedComponent = await context.IncludedComponents.FirstOrDefaultAsync(ic => ic.Description == incVm.Description);
                    if (includedComponent == null)
                    {
                        includedComponent = new IncludedComponent { Description = incVm.Description, CreatedAt = DateTime.UtcNow };
                        context.IncludedComponents.Add(includedComponent);
                        await context.SaveChangesAsync();
                    }
                    entity.IncludedComponents.Add(includedComponent);
                }
            }

            // RepairPositions zuordnen oder anlegen
            entity.RepairPositions = new List<RepairPosition>();
            if (vm.RepairPositions != null)
            {
                foreach (var posVm in vm.RepairPositions)
                {
                    var repairPosition = await context.RepairPositions.FirstOrDefaultAsync(rp => rp.Description == posVm.Description);
                    if (repairPosition == null)
                    {
                        repairPosition = new RepairPosition
                        {
                            Description = posVm.Description,
                            Artikelnummer = posVm.Artikelnummer,
                            SortOrder = posVm.SortOrder,
                            CreatedAt = DateTime.UtcNow
                        };
                        context.RepairPositions.Add(repairPosition);
                        await context.SaveChangesAsync();
                    }
                    entity.RepairOrderRepairPositions.Add(new RepairOrderRepairPosition { RepairOrder = entity, RepairPosition = repairPosition, Quantity = posVm.Quantity });
                }
            }

            // Employees zuordnen
            entity.Employees = new List<Employee>();
            if (vm.Employees != null)
            {
                foreach (var empVm in vm.Employees)
                {
                    var employee = await context.Set<Employee>().FirstOrDefaultAsync(e => e.Id == empVm.Id);
                    if (employee != null)
                    {
                        entity.Employees.Add(employee);
                    }
                }
            }

            context.RepairOrders.Add(entity);
            await context.SaveChangesAsync();

            var resultVm = _mapper.Map<RepairOrder, RepairOrderVm>(entity);
            return ServiceResponse.Success(resultVm);
        }

        public override async Task<ServiceResponse<RepairOrderVm>> UpdateAsync(object id, RepairOrderVm vm)
        {
            using var context = _contextFactory.CreateDbContext();

            var entity = await context.RepairOrders
                .Include(r => r.Defectives)
                .Include(r => r.IncludedComponents)
                .Include(r => r.RepairOrderRepairPositions)
                    .ThenInclude(rp => rp.RepairPosition)
                .Include(r => r.Employees)
                .FirstOrDefaultAsync(r => r.Id == (int)id);

            if (entity == null)
                return ServiceResponse.Failure<RepairOrderVm>("Nicht gefunden");

            // --- Statushistorie aktualisieren ---
            if (entity.RepairOrderStatusId != vm.RepairOrderStatusId)
            {
                context.RepairOrderStatusHistories.Add(new RepairOrderStatusHistory
                {
                    RepairOrderId = entity.Id,
                    RepairOrderStatusId = vm.RepairOrderStatusId,
                    ChangedAt = DateTime.UtcNow
                });
            }

            // Update Haupt-Entity
            _mapper.Map(vm, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            // --- Defectives synchronisieren ---
            var newDefectives = new List<Defective>();
            if (vm.Defectives != null)
            {
                foreach (var defVm in vm.Defectives)
                {
                    var defective = await context.Defectives.FirstOrDefaultAsync(d => d.Description == defVm.Description);
                    if (defective == null)
                    {
                        defective = new Defective { Description = defVm.Description, CreatedAt = DateTime.UtcNow };
                        context.Defectives.Add(defective);
                        await context.SaveChangesAsync();
                    }
                    newDefectives.Add(defective);
                }
            }
            // Entfernte Defectives löschen
            entity.Defectives.Clear();
            foreach (var d in newDefectives)
                entity.Defectives.Add(d);



            // --- Included Components synchronisieren ---
            var newIncludeComponents = new List<IncludedComponent>();
            if (vm.IncludedComponents != null)
            {
                foreach (var incVm in vm.IncludedComponents)
                {
                    var includedComponent = await context.IncludedComponents.FirstOrDefaultAsync(ic => ic.Description == incVm.Description);
                    if (includedComponent == null)
                    {
                        includedComponent = new IncludedComponent { Description = incVm.Description, CreatedAt = DateTime.UtcNow };
                        context.IncludedComponents.Add(includedComponent);
                        await context.SaveChangesAsync();
                    }
                    newIncludeComponents.Add(includedComponent);
                }
            }
            // Entfernte Included Components löschen
            entity.IncludedComponents.Clear();
            foreach (var d in newIncludeComponents)
                entity.IncludedComponents.Add(d);



            // --- RepairPositions synchronisieren ---
            // Entfernte Positionen löschen
            var vmPositions = vm.RepairPositions ?? new List<RepairPositionVm>();
            var toRemove = entity.RepairOrderRepairPositions
                .Where(rp => !vmPositions.Any(vp => vp.Description == rp.RepairPosition.Description))
                .ToList();
            foreach (var rp in toRemove)
                entity.RepairOrderRepairPositions.Remove(rp);

            // Hinzufügen/Aktualisieren
            foreach (var posVm in vmPositions)
            {
                var repairPosition = await context.RepairPositions.FirstOrDefaultAsync(rp => rp.Description == posVm.Description);
                if (repairPosition == null)
                {
                    repairPosition = new RepairPosition
                    {
                        Description = posVm.Description,
                        Artikelnummer = posVm.Artikelnummer,
                        SortOrder = posVm.SortOrder,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.RepairPositions.Add(repairPosition);
                    await context.SaveChangesAsync();
                }

                var existing = entity.RepairOrderRepairPositions
                    .FirstOrDefault(rp => rp.RepairPosition.Description == posVm.Description);

                if (existing == null)
                {
                    entity.RepairOrderRepairPositions.Add(new RepairOrderRepairPosition
                    {
                        RepairOrder = entity,
                        RepairPosition = repairPosition,
                        Quantity = posVm.Quantity
                    });
                }
                else
                {
                    existing.Quantity = posVm.Quantity;
                }
            }

            // --- Employees synchronisieren ---
            var newEmployees = new List<Employee>();
            if (vm.Employees != null)
            {
                foreach (var empVm in vm.Employees)
                {
                    var employee = await context.Set<Employee>().FirstOrDefaultAsync(e => e.Id == empVm.Id);
                    if (employee != null)
                    {
                        newEmployees.Add(employee);
                    }
                }
            }
            entity.Employees.Clear();
            foreach (var e in newEmployees)
                entity.Employees.Add(e);



            await context.SaveChangesAsync();

            var resultVm = _mapper.Map<RepairOrder, RepairOrderVm>(entity);
            return ServiceResponse.Success(resultVm);
        }

        public override async Task<ServiceResponse<RepairOrderVm>> GetByIdAsync(object id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entity = await context.RepairOrders
                    .Include(i => i.Defectives)
                    .Include(i => i.IncludedComponents)
                    .Include(i => i.RepairPositions)
                    .Include(i => i.RepairOrderStatus)
                    .Where(f => f.Id == (int)id)
                    .FirstOrDefaultAsync();
                if (entity == null)
                    return ServiceResponse.Failure<RepairOrderVm>("Nicht gefunden");
                var vm = _mapper.Map<RepairOrder, RepairOrderVm>(entity);
                return ServiceResponse.Success(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetByIdAsync");
                NotifyError("Fehler beim Abrufen des Datensatzes");
                return ServiceResponse.Failure<RepairOrderVm>("Fehler beim Abrufen des Datensatzes", ex);
            }
        }
    }
}
