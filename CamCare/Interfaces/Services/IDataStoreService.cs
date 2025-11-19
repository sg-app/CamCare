using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    public interface IDataStoreService : IDbService<DataStore, DataStoreVm>
    {
        Task<ServiceResponse<List<DataStoreVm>>> GetFromRepairOrderAsync(int repairOrderId);
    }
}
