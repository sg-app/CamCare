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
                .Include(i => i.DeliveryAddress)
                .Include(i => i.Employees)
                .Include(i => i.Defectives)
                .Include(i => i.Works)
                .Include(i => i.IncludedComponents)
                .Include(i => i.RepairOrderRepairPositions)
                    .ThenInclude(rp => rp.RepairPosition)
                .Include(i => i.EmailAddresses);

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
                    .Include(i => i.Works)
                    .Include(i => i.IncludedComponents)
                    .Include(i => i.RepairOrderRepairPositions)
                        .ThenInclude(rp => rp.RepairPosition)
                    .Include(i => i.DeliveryAddress)
                    .Include(i => i.EmailAddresses)
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

                // Lieferanschrift zuordnen bzw. neu zum Kunden speichern.
                await ApplyDeliveryAddressAsync(context, vm, entity);

                // E-Mail-Adressen zuordnen bzw. neu zum Kunden speichern.
                await ApplyEmailAddressesAsync(context, vm, entity);

                entity.Defectives = new List<Defective>();
                entity.Works = new List<Work>();
                entity.IncludedComponents = new List<IncludedComponent>();
                entity.Employees = new List<Employee>();
                entity.RepairOrderRepairPositions = new List<RepairOrderRepairPosition>();

                // Beim Anlegen eines Auftrags entspricht der Statuszeitstempel dem initialen Status.
                entity.CurrentStateAt = DateTime.UtcNow;

                // Wird der Auftrag direkt mit dem als "Eingetroffen" markierten Status angelegt,
                // wird der Eingetroffen-Zeitstempel auf den aktuellen Zeitpunkt gesetzt.
                if (await context.RepairOrderStatuses
                        .AsNoTracking()
                        .AnyAsync(s => s.Id == entity.RepairOrderStatusId && s.IsArrived))
                {
                    entity.ArrivedAt = DateTime.UtcNow;
                }

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

                // Works zuordnen oder anlegen
                if (vm.Works != null)
                {
                    var descriptions = vm.Works
                        .Select(d => d.Description)
                        .Where(d => !string.IsNullOrWhiteSpace(d))
                        .Distinct()
                        .ToList();

                    var worksByDescription = await context.Works
                        .Where(d => descriptions.Contains(d.Description))
                        .ToDictionaryAsync(d => d.Description, d => d);

                    foreach (var defVm in vm.Works)
                    {
                        if (string.IsNullOrWhiteSpace(defVm.Description))
                            continue;

                        if (!worksByDescription.TryGetValue(defVm.Description, out var work))
                        {
                            work = new Work { Description = defVm.Description, CreatedAt = DateTime.UtcNow };
                            context.Works.Add(work);
                            worksByDescription[defVm.Description] = work;
                        }

                        entity.Works.Add(work);
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
                    .Include(i => i.Works)
                    .Include(r => r.IncludedComponents)
                    .Include(r => r.RepairOrderRepairPositions)
                        .ThenInclude(rp => rp.RepairPosition)
                    .Include(r => r.Employees)
                    .Include(r => r.EmailAddresses)
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
                var statusChanged = entity.RepairOrderStatusId != vm.RepairOrderStatusId;
                RepairOrderStatus? newStatus = null;
                if (statusChanged)
                {
                    // Statusnamen und Farben zum Zeitpunkt der Änderung als Snapshot sichern,
                    // damit die Historie auch nach dem Löschen eines Status nachvollziehbar bleibt.
                    newStatus = await context.RepairOrderStatuses
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

                // Lieferanschrift zuordnen bzw. neu zum Kunden speichern.
                await ApplyDeliveryAddressAsync(context, vm, entity);

                // E-Mail-Adressen zuordnen bzw. neu zum Kunden speichern.
                await ApplyEmailAddressesAsync(context, vm, entity);

                entity.UpdatedAt = DateTime.UtcNow;

                // Wird der Status geändert, wird der Zeitstempel der aktuellen Statusänderung gesetzt.
                if (statusChanged)
                    entity.CurrentStateAt = DateTime.UtcNow;

                // Wird der Auftrag in den als "Eingetroffen" markierten Status gesetzt,
                // wird der Eingetroffen-Zeitstempel auf den aktuellen Zeitpunkt gesetzt.
                if (statusChanged && newStatus?.IsArrived == true)
                    entity.ArrivedAt = DateTime.UtcNow;

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

                // --- Works synchronisieren ---
                var vmWorks = vm.Works ?? new List<WorkVm>();
                var workDescriptions = vmWorks
                    .Select(d => d.Description)
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .Distinct()
                    .ToList();

                var worksByDescription = await context.Works
                    .Where(d => workDescriptions.Contains(d.Description))
                    .ToDictionaryAsync(d => d.Description, d => d);

                var newWorks = new List<Work>();
                foreach (var workVm in vmWorks)
                {
                    if (string.IsNullOrWhiteSpace(workVm.Description))
                        continue;

                    if (!worksByDescription.TryGetValue(workVm.Description, out var work))
                    {
                        work = new Work { Description = workVm.Description, CreatedAt = DateTime.UtcNow };
                        context.Works.Add(work);
                        worksByDescription[workVm.Description] = work;
                    }

                    newWorks.Add(work);
                }

                entity.Works.Clear();
                foreach (var work in newWorks)
                    entity.Works.Add(work);

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

        /// <summary>
        /// Ordnet dem Auftrag die ausgewählte Lieferanschrift zu. Wurde im Formular eine
        /// neue Anschrift erfasst, wird diese für den Kunden gespeichert. Eine bereits
        /// vorhandene, identische Anschrift des Kunden wird wiederverwendet, um Duplikate
        /// zu vermeiden.
        /// </summary>
        private static async Task ApplyDeliveryAddressAsync(IAppDbContext context, RepairOrderVm vm, RepairOrder entity)
        {
            var selected = vm.DeliveryAddress;

            // Keine Anschrift erfasst → die Zuordnung aus dem ViewModel (bzw. keine
            // Zuordnung) bleibt unverändert bestehen.
            if (selected is null)
                return;

            // Eine bereits gespeicherte Lieferanschrift wurde ausgewählt.
            if (selected.Id > 0)
            {
                entity.DeliveryAddressId = selected.Id;
                return;
            }

            // Unvollständig erfasste Anschriften werden nicht gespeichert.
            if (string.IsNullOrWhiteSpace(selected.Recipient)
                || string.IsNullOrWhiteSpace(selected.Street)
                || string.IsNullOrWhiteSpace(selected.PostalCode)
                || string.IsNullOrWhiteSpace(selected.City))
            {
                return;
            }

            var recipient = selected.Recipient.Trim();
            var street = selected.Street.Trim();
            var postalCode = selected.PostalCode.Trim();
            var city = selected.City.Trim();

            // Existiert die Anschrift beim Kunden bereits, wird sie wiederverwendet.
            var existing = await context.DeliveryAddresses
                .FirstOrDefaultAsync(a => a.CustomerId == entity.CustomerId
                                          && a.Recipient == recipient
                                          && a.Street == street
                                          && a.PostalCode == postalCode
                                          && a.City == city);

            if (existing is not null)
            {
                entity.DeliveryAddressId = existing.Id;
                return;
            }

            // Neue Lieferanschrift zum Kunden speichern. Die Id wird beim Speichern des
            // Auftrags über die Navigation gesetzt.
            entity.DeliveryAddress = new DeliveryAddress
            {
                CustomerId = entity.CustomerId,
                Recipient = recipient,
                ContactPerson = string.IsNullOrWhiteSpace(selected.ContactPerson) ? null : selected.ContactPerson.Trim(),
                Street = street,
                PostalCode = postalCode,
                City = city,
                Country = string.IsNullOrWhiteSpace(selected.Country) ? null : selected.Country.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Ordnet dem Auftrag die ausgewählten E-Mail-Adressen zu. Wurde im Formular eine
        /// neue Adresse erfasst, wird diese für den Kunden gespeichert. Eine bereits beim
        /// Kunden hinterlegte, identische Adresse wird wiederverwendet, um Duplikate zu
        /// vermeiden. Die Auswahl im Formular ist maßgeblich – nicht (mehr) ausgewählte
        /// Adressen werden vom Auftrag entfernt (die Adresse beim Kunden bleibt erhalten).
        /// </summary>
        private static async Task ApplyEmailAddressesAsync(IAppDbContext context, RepairOrderVm vm, RepairOrder entity)
        {
            var selected = vm.EmailAddresses?.ToList() ?? [];

            // Die Zuordnung erfolgt ausschließlich über die Auswahl im Formular.
            entity.EmailAddresses.Clear();

            if (selected.Count == 0 || entity.CustomerId <= 0)
                return;

            // Bereits beim Kunden hinterlegte Adressen einmalig laden (Groß-/Kleinschreibung
            // spielt bei E-Mail-Adressen keine Rolle).
            var customerAddresses = await context.EmailAddresses
                .Where(a => a.CustomerId == entity.CustomerId && a.ArchivedAt == null)
                .ToListAsync();

            var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var selectedAddress in selected)
            {
                var address = selectedAddress.Address?.Trim();
                if (string.IsNullOrWhiteSpace(address))
                    continue;

                var saved = selectedAddress.Id > 0
                    ? customerAddresses.FirstOrDefault(a => a.Id == selectedAddress.Id)
                    : customerAddresses.FirstOrDefault(a => string.Equals(a.Address, address, StringComparison.OrdinalIgnoreCase));

                if (saved is null)
                {
                    // Neue E-Mail-Adresse zum Kunden speichern. Die Zuordnung zum Auftrag
                    // erfolgt über die Navigation und wird beim Speichern gesetzt.
                    saved = new EmailAddress
                    {
                        CustomerId = entity.CustomerId,
                        Address = address,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.EmailAddresses.Add(saved);
                    customerAddresses.Add(saved);
                }

                if (added.Add(saved.Address.Trim()))
                    entity.EmailAddresses.Add(saved);
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
                    .Include(i => i.Works)
                    .Include(i => i.IncludedComponents)
                    .Include(i => i.RepairOrderRepairPositions)
                        .ThenInclude(rp => rp.RepairPosition)
                    .Include(i => i.DeliveryAddress)
                    .Include(i => i.EmailAddresses)
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
