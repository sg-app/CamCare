namespace CamCare.Models
{
    public class UserGroupVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>Nur in der Admin-UI (Benutzergruppen-Seite) verwendet.</summary>
        public string? NewPassword { get; set; }

        /// <summary>Nur in der Admin-UI (Benutzergruppen-Seite) verwendet.</summary>
        public string? ConfirmPassword { get; set; }
    }
}
