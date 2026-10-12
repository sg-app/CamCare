using CamCare.Models;
using Radzen;

namespace CamCare.Interfaces.Services
{
    public interface IKrdDataService
    {
        Task<ServiceResponse<Paginated<Krd_DataVm>>> GetAllAsync(LoadDataArgs args);
        Task<ServiceResponse<List<Krd_DataVm>>> GetBySerialNumberAsync(string serialNumber);
    }
}
