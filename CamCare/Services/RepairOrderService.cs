using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;

namespace CamCare.Services
{
    public class RepairOrderService : DbService<RepairOrder, RepairOrderVm>, IRepairOrderService
    {
        private readonly IAmicronDataService _amicronDataService;
        private readonly IRepairOrderChangeBroadcaster _changeBroadcaster;

        public RepairOrderService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<RepairOrderService> logger, NotificationService notificationService, IAmicronDataService amicronDataService, IRepairOrderChangeBroadcaster changeBroadcaster)
            : base(contextFactory, mapper, logger, notificationService)
        {
            _amicronDataService = amicronDataService;
            _changeBroadcaster = changeBroadcaster;
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

            var customerIds = vms
                .Where(vm => vm.CustomerId.HasValue)
                .Select(vm => vm.CustomerId!.Value)
                .Distinct()
                .ToList();

            if (customerIds.Count > 0)
            {
                var customersResponse = await _amicronDataService.GetAddressesByCustomerIdsAsync(customerIds);
                if (customersResponse.Success && customersResponse.Data != null)
                {
                    var customersById = customersResponse.Data.ToDictionary(customer => customer.LfdNr);
                    foreach (var vm in vms)
                    {
                        if (!vm.CustomerId.HasValue)
                            continue;

                        if (customersById.TryGetValue(vm.CustomerId.Value, out var customer))
                        {
                            vm.Customer = customer;
                        }
                    }
                }
            }

            var paginated = new Paginated<RepairOrderVm>
            {
                Items = vms,
                TotalCount = totalCount
            };
            return ServiceResponse.Success(paginated);
        }

        /// <summary>
        /// Liefert alle Aufträge (auch archivierte) zu einer Seriennummer für das Logbuch.
        /// </summary>
        public async Task<ServiceResponse<List<RepairOrderVm>>> GetBySerialNumberAsync(string serialNumber)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var items = await context.RepairOrders
                    .AsNoTracking()
                    .Where(f => f.SerialNumber == serialNumber)
                    .Include(i => i.RepairOrderStatus)
                    .Include(i => i.RepairOrderStatusHistory)
                        .ThenInclude(i => i.RepairOrderStatus)
                    .Include(i => i.LogisticProvider)
                    .Include(i => i.Employees)
                    .Include(i => i.Defectives)
                    .Include(i => i.IncludedComponents)
                    .Include(i => i.RepairOrderRepairPositions)
                        .ThenInclude(rp => rp.RepairPosition)
                    .OrderByDescending(i => i.ArrivedAt)
                    .ToListAsync();

                var vms = items.Select(_mapper.Map<RepairOrder, RepairOrderVm>).ToList();

                // Kundendaten aus dem Amicron-System anreichern (wie in GetAllAsync)
                var customerIds = vms
                    .Where(vm => vm.CustomerId.HasValue)
                    .Select(vm => vm.CustomerId!.Value)
                    .Distinct()
                    .ToList();

                if (customerIds.Count > 0)
                {
                    var customersResponse = await _amicronDataService.GetAddressesByCustomerIdsAsync(customerIds);
                    if (customersResponse.Success && customersResponse.Data != null)
                    {
                        var customersById = customersResponse.Data.ToDictionary(customer => customer.LfdNr);
                        foreach (var vm in vms)
                        {
                            if (!vm.CustomerId.HasValue)
                                continue;

                            if (customersById.TryGetValue(vm.CustomerId.Value, out var customer))
                            {
                                vm.Customer = customer;
                            }
                        }
                    }
                }

                return ServiceResponse.Success(vms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Aufträge für Seriennummer {SerialNumber}", serialNumber);
                NotifyError("Fehler beim Abrufen der Aufträge");
                return ServiceResponse.Failure<List<RepairOrderVm>>("Fehler beim Abrufen der Aufträge", ex);
            }
        }

        /// <summary>
        /// Liefert alle Seriennummern, für die aktuell mindestens ein offener (nicht abgeschlossener)
        /// Reparaturauftrag existiert. Wird verwendet, um Seriennummern in Listenansichten zu kennzeichnen.
        /// </summary>
        public async Task<ServiceResponse<HashSet<string>>> GetOpenOrderSerialNumbersAsync()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var serialNumbers = await context.RepairOrders
                    .AsNoTracking()
                    .Where(f => !f.RepairOrderStatus.IsOrderClose)
                    .Where(f => f.SerialNumber != null)
                    .Select(f => f.SerialNumber!.Trim())
                    .Distinct()
                    .ToListAsync();

                return ServiceResponse.Success(new HashSet<string>(serialNumbers, StringComparer.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Ermitteln der Seriennummern mit offenen Aufträgen");
                NotifyError("Fehler beim Ermitteln der Seriennummern mit offenen Aufträgen");
                return ServiceResponse.Failure<HashSet<string>>("Fehler beim Ermitteln der Seriennummern mit offenen Aufträgen", ex);
            }
        }

        public override async Task<ServiceResponse<RepairOrderVm>> CreateAsync(RepairOrderVm vm)
        {
            using var context = _contextFactory.CreateDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Verhindern, dass für eine Seriennummer ein zweiter offener Auftrag erstellt wird.
                if (!string.IsNullOrWhiteSpace(vm.CameraSerialNumber)
                    && await context.RepairOrders
                        .AnyAsync(o => o.SerialNumber != null
                                       && o.SerialNumber.Trim().ToLower() == vm.CameraSerialNumber.Trim().ToLower()
                                       && !o.RepairOrderStatus.IsOrderClose))
                {
                    await transaction.RollbackAsync();
                    return ServiceResponse.Failure<RepairOrderVm>(
                        $"Für die Seriennummer {vm.CameraSerialNumber} existiert bereits ein offener Auftrag. Es kann kein weiterer Auftrag erstellt werden.");
                }

                var entity = new RepairOrder();
                _mapper.Map(vm, entity);

                entity.Defectives = new List<Defective>();
                entity.IncludedComponents = new List<IncludedComponent>();
                entity.Employees = new List<Employee>();
                entity.RepairOrderRepairPositions = new List<RepairOrderRepairPosition>();

                // Defectives zuordnen oder anlegen
                if (vm.Defectives != null)
                {
                    var descriptions = vm.Defectives
                        .Select(d => d.Description)
                        .Where(d => !string.IsNullOrWhiteSpace(d))
                        .Distinct()
                        .ToList();

                    var defectivesByDescription = await context.Defectives
                        .Where(d => descriptions.Contains(d.Description))
                        .ToDictionaryAsync(d => d.Description, d => d);

                    foreach (var defVm in vm.Defectives)
                    {
                        if (string.IsNullOrWhiteSpace(defVm.Description))
                            continue;

                        if (!defectivesByDescription.TryGetValue(defVm.Description, out var defective))
                        {
                            defective = new Defective { Description = defVm.Description, CreatedAt = DateTime.UtcNow };
                            context.Defectives.Add(defective);
                            defectivesByDescription[defVm.Description] = defective;
                        }

                        entity.Defectives.Add(defective);
                    }
                }

                // Included Components zuordnen oder anlegen
                if (vm.IncludedComponents != null)
                {
                    var descriptions = vm.IncludedComponents
                        .Select(i => i.Description)
                        .Where(d => !string.IsNullOrWhiteSpace(d))
                        .Distinct()
                        .ToList();

                    var includedComponentsByDescription = await context.IncludedComponents
                        .Where(ic => descriptions.Contains(ic.Description))
                        .ToDictionaryAsync(ic => ic.Description, ic => ic);

                    foreach (var incVm in vm.IncludedComponents)
                    {
                        if (string.IsNullOrWhiteSpace(incVm.Description))
                            continue;

                        if (!includedComponentsByDescription.TryGetValue(incVm.Description, out var includedComponent))
                        {
                            includedComponent = new IncludedComponent { Description = incVm.Description, CreatedAt = DateTime.UtcNow };
                            context.IncludedComponents.Add(includedComponent);
                            includedComponentsByDescription[incVm.Description] = includedComponent;
                        }

                        entity.IncludedComponents.Add(includedComponent);
                    }
                }

                // RepairPositions zuordnen oder anlegen
                if (vm.RepairPositions != null)
                {
                    var descriptions = vm.RepairPositions
                        .Select(rp => rp.Description)
                        .Where(d => !string.IsNullOrWhiteSpace(d))
                        .Distinct()
                        .ToList();

                    var repairPositionsByDescription = await context.RepairPositions
                        .Where(rp => descriptions.Contains(rp.Description))
                        .ToDictionaryAsync(rp => rp.Description, rp => rp);

                    foreach (var posVm in vm.RepairPositions)
                    {
                        if (string.IsNullOrWhiteSpace(posVm.Description))
                            continue;

                        if (!repairPositionsByDescription.TryGetValue(posVm.Description, out var repairPosition))
                        {
                            repairPosition = new RepairPosition
                            {
                                Description = posVm.Description,
                                Artikelnummer = posVm.Artikelnummer,
                                SortOrder = posVm.SortOrder,
                                CreatedAt = DateTime.UtcNow
                            };
                            context.RepairPositions.Add(repairPosition);
                            repairPositionsByDescription[posVm.Description] = repairPosition;
                        }

                        entity.RepairOrderRepairPositions.Add(new RepairOrderRepairPosition
                        {
                            RepairOrder = entity,
                            RepairPosition = repairPosition,
                            Quantity = posVm.Quantity
                        });
                    }
                }

                // Employees zuordnen
                if (vm.Employees != null)
                {
                    var employeeIds = vm.Employees.Select(e => e.Id).Distinct().ToList();
                    var employees = await context.Set<Employee>()
                        .Where(e => employeeIds.Contains(e.Id))
                        .ToListAsync();

                    foreach (var employee in employees)
                    {
                        entity.Employees.Add(employee);
                    }
                }

                context.RepairOrders.Add(entity);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _changeBroadcaster.PublishAsync(new RepairOrderChangedEvent(entity.Id, RepairOrderChangeType.Created));

                var resultVm = _mapper.Map<RepairOrder, RepairOrderVm>(entity);
                return ServiceResponse.Success(resultVm);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public override async Task<ServiceResponse<RepairOrderVm>> UpdateAsync(object id, RepairOrderVm vm)
        {
            using var context = _contextFactory.CreateDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var entity = await context.RepairOrders
                    .Include(r => r.Defectives)
                    .Include(r => r.IncludedComponents)
                    .Include(r => r.RepairOrderRepairPositions)
                        .ThenInclude(rp => rp.RepairPosition)
                    .Include(r => r.Employees)
                    .FirstOrDefaultAsync(r => r.Id == (int)id);

                if (entity == null)
                    return ServiceResponse.Failure<RepairOrderVm>("Nicht gefunden");

                // Verhindern, dass die Seriennummer auf eine Seriennummer geändert wird,
                // für die bereits ein anderer offener Auftrag existiert.
                if (!string.IsNullOrWhiteSpace(vm.CameraSerialNumber)
                    && await context.RepairOrders
                        .AnyAsync(o => o.Id != entity.Id
                                       && o.SerialNumber != null
                                       && o.SerialNumber.Trim().ToLower() == vm.CameraSerialNumber.Trim().ToLower()
                                       && !o.RepairOrderStatus.IsOrderClose))
                {
                    await transaction.RollbackAsync();
                    return ServiceResponse.Failure<RepairOrderVm>(
                        $"Für die Seriennummer {vm.CameraSerialNumber} existiert bereits ein anderer offener Auftrag. Die Seriennummer kann nicht verwendet werden.");
                }

                // --- Statushistorie aktualisieren ---
                if (entity.RepairOrderStatusId != vm.RepairOrderStatusId)
                {
                    // Statusnamen und Farben zum Zeitpunkt der Änderung als Snapshot sichern,
                    // damit die Historie auch nach dem Löschen eines Status nachvollziehbar bleibt.
                    var newStatus = await context.RepairOrderStatuses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Id == vm.RepairOrderStatusId);

                    context.RepairOrderStatusHistories.Add(new RepairOrderStatusHistory
                    {
                        RepairOrderId = entity.Id,
                        RepairOrderStatusId = vm.RepairOrderStatusId,
                        StatusName = newStatus?.Name,
                        BackgroundColor = newStatus?.BackgroundColor,
                        FontColor = newStatus?.FontColor,
                        ChangedAt = DateTime.UtcNow
                    });
                }

                // Update Haupt-Entity
                _mapper.Map(vm, entity);
                entity.UpdatedAt = DateTime.UtcNow;

                // --- Defectives synchronisieren ---
                var vmDefectives = vm.Defectives ?? new List<DefectiveVm>();
                var defectiveDescriptions = vmDefectives
                    .Select(d => d.Description)
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .Distinct()
                    .ToList();

                var defectivesByDescription = await context.Defectives
                    .Where(d => defectiveDescriptions.Contains(d.Description))
                    .ToDictionaryAsync(d => d.Description, d => d);

                var newDefectives = new List<Defective>();
                foreach (var defVm in vmDefectives)
                {
                    if (string.IsNullOrWhiteSpace(defVm.Description))
                        continue;

                    if (!defectivesByDescription.TryGetValue(defVm.Description, out var defective))
                    {
                        defective = new Defective { Description = defVm.Description, CreatedAt = DateTime.UtcNow };
                        context.Defectives.Add(defective);
                        defectivesByDescription[defVm.Description] = defective;
                    }

                    newDefectives.Add(defective);
                }

                entity.Defectives.Clear();
                foreach (var defective in newDefectives)
                    entity.Defectives.Add(defective);

                // --- Included Components synchronisieren ---
                var vmIncludedComponents = vm.IncludedComponents ?? new List<IncludedComponentVm>();
                var includedDescriptions = vmIncludedComponents
                    .Select(i => i.Description)
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .Distinct()
                    .ToList();

                var includedByDescription = await context.IncludedComponents
                    .Where(ic => includedDescriptions.Contains(ic.Description))
                    .ToDictionaryAsync(ic => ic.Description, ic => ic);

                var newIncludedComponents = new List<IncludedComponent>();
                foreach (var incVm in vmIncludedComponents)
                {
                    if (string.IsNullOrWhiteSpace(incVm.Description))
                        continue;

                    if (!includedByDescription.TryGetValue(incVm.Description, out var includedComponent))
                    {
                        includedComponent = new IncludedComponent { Description = incVm.Description, CreatedAt = DateTime.UtcNow };
                        context.IncludedComponents.Add(includedComponent);
                        includedByDescription[incVm.Description] = includedComponent;
                    }

                    newIncludedComponents.Add(includedComponent);
                }

                entity.IncludedComponents.Clear();
                foreach (var includedComponent in newIncludedComponents)
                    entity.IncludedComponents.Add(includedComponent);

                // --- RepairPositions synchronisieren ---
                var vmPositions = vm.RepairPositions ?? new List<RepairPositionVm>();
                var positionDescriptions = vmPositions
                    .Select(p => p.Description)
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .Distinct()
                    .ToList();

                var repairPositionsByDescription = await context.RepairPositions
                    .Where(rp => positionDescriptions.Contains(rp.Description))
                    .ToDictionaryAsync(rp => rp.Description, rp => rp);

                var toRemove = entity.RepairOrderRepairPositions
                    .Where(rp => !vmPositions.Any(vp => vp.Description == rp.RepairPosition.Description))
                    .ToList();

                foreach (var rp in toRemove)
                    entity.RepairOrderRepairPositions.Remove(rp);

                foreach (var posVm in vmPositions)
                {
                    if (string.IsNullOrWhiteSpace(posVm.Description))
                        continue;

                    if (!repairPositionsByDescription.TryGetValue(posVm.Description, out var repairPosition))
                    {
                        repairPosition = new RepairPosition
                        {
                            Description = posVm.Description,
                            Artikelnummer = posVm.Artikelnummer,
                            SortOrder = posVm.SortOrder,
                            CreatedAt = DateTime.UtcNow
                        };
                        context.RepairPositions.Add(repairPosition);
                        repairPositionsByDescription[posVm.Description] = repairPosition;
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
                var employeeIds = (vm.Employees ?? new List<EmployeeVm>())
                    .Select(e => e.Id)
                    .Distinct()
                    .ToList();

                var newEmployees = await context.Set<Employee>()
                    .Where(e => employeeIds.Contains(e.Id))
                    .ToListAsync();

                entity.Employees.Clear();
                foreach (var employee in newEmployees)
                    entity.Employees.Add(employee);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _changeBroadcaster.PublishAsync(new RepairOrderChangedEvent(entity.Id, RepairOrderChangeType.Updated));

                var resultVm = _mapper.Map<RepairOrder, RepairOrderVm>(entity);
                return ServiceResponse.Success(resultVm);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public override async Task<ServiceResponse<RepairOrderVm>> GetByIdAsync(object id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entity = await context.RepairOrders
                    .AsNoTracking()
                    .Include(i => i.RepairOrderStatus)
                    .Include(i => i.RepairOrderStatusHistory)
                        .ThenInclude(i => i.RepairOrderStatus)
                    .Include(i => i.LogisticProvider)
                    .Include(i => i.Employees)
                    .Include(i => i.Defectives)
                    .Include(i => i.IncludedComponents)
                    .Include(i => i.RepairOrderRepairPositions)
                        .ThenInclude(rp => rp.RepairPosition)
                    .Where(f => f.Id == (int)id)
                    .FirstOrDefaultAsync();
                if (entity == null)
                    return ServiceResponse.Failure<RepairOrderVm>("Nicht gefunden");

                var vm = _mapper.Map<RepairOrder, RepairOrderVm>(entity);

                // Kundendaten aus dem Amicron-System anreichern (wie in GetAllAsync).
                if (vm.CustomerId.HasValue)
                {
                    var customersResponse = await _amicronDataService.GetAddressesByCustomerIdsAsync([vm.CustomerId.Value]);
                    if (customersResponse.Success && customersResponse.Data != null)
                    {
                        var customer = customersResponse.Data.FirstOrDefault(c => c.LfdNr == vm.CustomerId.Value);
                        if (customer != null)
                            vm.Customer = customer;
                    }
                }

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
