using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    /// <summary>
    /// Eine feste Benutzergruppe (admin, buero, werkstatt) mit eigenem Passwort.
    /// Es gibt keine einzelnen Benutzerkonten.
    /// </summary>
    public class UserGroup : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
