using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Radzen;

namespace CamCare.Services
{
    /// <summary>
    /// Verwaltet die E-Mail-Vorlagen (eine Vorlage je Status bzw. mehrere Vorlagen je Status).
    /// </summary>
    public class MailTemplateService(
        IAppDbContextFactory contextFactory,
        IMapper mapper,
        ILogger<MailTemplateService> logger,
        NotificationService notificationService)
        : DbService<MailTemplate, MailTemplateVm>(contextFactory, mapper, logger, notificationService), IMailTemplateService
    {
    }
}
