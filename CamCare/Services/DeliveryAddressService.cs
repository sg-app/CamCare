using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace CamCare.Services
{
    public class DeliveryAddressService : DbService<DeliveryAddress, DeliveryAddressVm>, IDeliveryAddressService
    {
        public DeliveryAddressService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<DeliveryAddressService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

        public async Task<ServiceResponse<List<DeliveryAddressVm>>> GetByCustomerIdAsync(int customerId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entities = await context.DeliveryAddresses
                    .AsNoTracking()
                    .Where(a => a.CustomerId == customerId && a.ArchivedAt == null)
                    .OrderBy(a => a.Recipient)
                    .ThenBy(a => a.City)
                    .ToListAsync();

                var viewModels = entities.Select(_mapper.Map<DeliveryAddress, DeliveryAddressVm>).ToList();

                return ServiceResponse.Success(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Lieferanschriften für Kunde {CustomerId}", customerId);
                NotifyError("Fehler beim Abrufen der Lieferanschriften");
                return ServiceResponse.Failure<List<DeliveryAddressVm>>("Fehler beim Abrufen der Lieferanschriften", ex);
            }
        }
    }
}
