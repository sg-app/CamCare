using CamCare.Domain;
using CamCare.Models;
using Radzen;
using System.Linq.Expressions;

namespace CamCare.Interfaces.Services
{
    public interface IRepairOrderService : IDbService<RepairOrder, RepairOrderVm>
    {
        Task<ServiceResponse<Paginated<RepairOrderVm>>> GetAllAsync(LoadDataArgs args, bool viewCurrentOrders, Expression<Func<RepairOrder, bool>>? predicate = null);
    }
}
