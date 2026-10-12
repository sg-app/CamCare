using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    public interface IUserGroupService
    {
        /// <summary>
        /// Legt die drei bekannten Benutzergruppen mit ihren Standard-Passwörtern an, falls sie fehlen.
        /// Wird beim Anwendungsstart aufgerufen.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Prüft, ob die angegebenen Anmeldedaten zu einer Benutzergruppe gehören.
        /// Bei Erfolg wird die Benutzergruppe (mit kanonischem Namen) zurückgegeben, sonst <c>null</c>.
        /// </summary>
        Task<UserGroup?> ValidateAsync(string groupName, string password);

        Task<ServiceResponse<List<UserGroupVm>>> GetAllAsync();

        Task<ServiceResponse<bool>> UpdatePasswordAsync(int id, string newPassword);
    }
}
