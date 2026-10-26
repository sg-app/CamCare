using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace CamCare.Services
{
    /// <summary>
    /// Lesezugriff auf das Protokoll der automatisch versendeten Status-E-Mails.
    /// </summary>
    public class RepairOrderMailLogService(
        IAppDbContextFactory contextFactory,
        IMapper mapper,
        ILogger<RepairOrderMailLogService> logger,
        NotificationService notificationService)
        : DbService<RepairOrderMailLog, RepairOrderMailLogVm>(contextFactory, mapper, logger, notificationService), IRepairOrderMailLogService
    {
    }
}
