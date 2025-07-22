using System.Threading.Tasks;
using CamCare.Domain;
using CamCare.Models;
using Radzen;

namespace CamCare.Interfaces.Services
{
    public interface IRepairOrderStatusService : IDbService<RepairOrderStatus, RepairOrderStatusVm>
    {
        // Hier können zusätzliche, spezifische Methoden ergänzt werden
    }
}
