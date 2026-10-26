using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Lesezugriff auf das Protokoll der automatisch versendeten Status-E-Mails.
    /// </summary>
    public interface IRepairOrderMailLogService : IDbService<RepairOrderMailLog, RepairOrderMailLogVm>
    {
    }
}
