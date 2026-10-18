using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    public interface IEmailAddressService : IDbService<EmailAddress, EmailAddressVm>
    {
        /// <summary>
        /// Liefert alle E-Mail-Adressen des angegebenen Kunden.
        /// </summary>
        Task<ServiceResponse<List<EmailAddressVm>>> GetByCustomerIdAsync(int customerId);
    }
}
