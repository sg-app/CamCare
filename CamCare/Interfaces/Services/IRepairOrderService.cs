using CamCare.Domain;
using CamCare.Models;
using Radzen;

namespace CamCare.Interfaces.Services
{
    public interface IRepairOrderService : IDbService<RepairOrder, RepairOrderVm>
    {
        Task<ServiceResponse<Paginated<RepairOrderVm>>> GetAllAsync(LoadDataArgs args, bool viewCurrentOrders);
    }
}
