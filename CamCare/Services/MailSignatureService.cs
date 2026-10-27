using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace CamCare.Services
{
    /// <summary>
    /// Verwaltet die E-Mail-Signatur, die an jede versendete E-Mail angehängt wird.
    /// </summary>
    public class MailSignatureService(
        IAppDbContextFactory contextFactory,
        IMapper mapper,
        ILogger<MailSignatureService> logger,
        NotificationService notificationService)
        : DbService<MailSignature, MailSignatureVm>(contextFactory, mapper, logger, notificationService), IMailSignatureService
    {
        public async Task<ServiceResponse<MailSignatureVm>> GetSignatureAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var entity = await context.MailSignatures
                    .AsNoTracking()
                    .OrderBy(s => s.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity is null)
                {
                    // Noch keine Signatur vorhanden: Standardinhalt zum Anlegen anbieten.
                    return ServiceResponse.Success(new MailSignatureVm
                    {
                        IsEnabled = false
                    });
                }

                return ServiceResponse.Success(_mapper.Map<MailSignature, MailSignatureVm>(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der E-Mail-Signatur.");
                NotifyError("Fehler beim Abrufen der E-Mail-Signatur");
                return ServiceResponse.Failure<MailSignatureVm>("Fehler beim Abrufen der E-Mail-Signatur", ex);
            }
        }
    }
}
