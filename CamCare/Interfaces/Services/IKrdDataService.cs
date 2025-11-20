using CamCare.Models;
using Radzen;

namespace CamCare.Interfaces.Services
{
    public interface IKrdDataService
    {
        Task<ServiceResponse<Paginated<Krd_DataVm>>> GetAllAsync(LoadDataArgs args);
    }
}
