using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    public interface IRepairOrderStatusService : IDbService<RepairOrderStatus, RepairOrderStatusVm>
    {
        Task<ServiceResponse<bool>> UpdateOrderAsync(IList<RepairOrderStatusVm> vms);

        /// <summary>
        /// Liefert, wie oft der Status in der Statushistorie bzw. als aktueller Status von Reparaturaufträgen verwendet wird.
        /// </summary>
        Task<ServiceResponse<RepairOrderStatusUsageVm>> GetUsageAsync(int id);
    }
}
