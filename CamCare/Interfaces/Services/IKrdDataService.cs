using CamCare.Domain;
using CamCare.Models;
using Radzen;
using System.Linq.Expressions;

namespace CamCare.Interfaces.Services
{
    public interface IKrdDataService
    {
        Task<ServiceResponse<Paginated<Krd_DataVm>>> GetAllAsync(LoadDataArgs args);
    }
}
