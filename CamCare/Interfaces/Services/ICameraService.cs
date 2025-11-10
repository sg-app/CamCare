using CamCare.Domain;
using CamCare.Models;
using Radzen;

namespace CamCare.Interfaces.Services
{
    public interface ICameraService : IDbService<Camera, CameraVm>
    {
        Task<ServiceResponse<Paginated<CameraVm>>> GetForDropdownAsync(LoadDataArgs args, int customerId);
    }
}
