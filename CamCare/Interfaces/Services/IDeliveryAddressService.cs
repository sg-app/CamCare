using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    public interface IDeliveryAddressService : IDbService<DeliveryAddress, DeliveryAddressVm>
    {
        /// <summary>
        /// Liefert alle Lieferanschriften des angegebenen Kunden.
        /// </summary>
        Task<ServiceResponse<List<DeliveryAddressVm>>> GetByCustomerIdAsync(int customerId);
    }
}
