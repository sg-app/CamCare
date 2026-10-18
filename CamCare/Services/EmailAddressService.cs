using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace CamCare.Services
{
    public class EmailAddressService : DbService<EmailAddress, EmailAddressVm>, IEmailAddressService
    {
        public EmailAddressService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<EmailAddressService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }

        public async Task<ServiceResponse<List<EmailAddressVm>>> GetByCustomerIdAsync(int customerId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entities = await context.EmailAddresses
                    .AsNoTracking()
                    .Where(a => a.CustomerId == customerId && a.ArchivedAt == null)
                    .OrderBy(a => a.Address)
                    .ToListAsync();

                var viewModels = entities.Select(_mapper.Map<EmailAddress, EmailAddressVm>).ToList();

                return ServiceResponse.Success(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der E-Mail-Adressen für Kunde {CustomerId}", customerId);
                NotifyError("Fehler beim Abrufen der E-Mail-Adressen");
                return ServiceResponse.Failure<List<EmailAddressVm>>("Fehler beim Abrufen der E-Mail-Adressen", ex);
            }
        }
    }
}
