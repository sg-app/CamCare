using System.ComponentModel.DataAnnotations;

namespace CamCare.Options
{
    /// <summary>
    /// Verschlüsselung der Verbindung zum SMTP-Server (Einstellung <c>Mail:Security</c> in der
    /// <c>appsettings.json</c>).
    /// </summary>
    public enum MailSecurityMode
    {
        [Display(Description = "Unverschlüsselt (nicht empfohlen)")]
        None = 0,

        [Display(Description = "STARTTLS (empfohlen, z. B. Port 587)")]
        StartTls = 1,

        [Display(Description = "SSL/TLS (z. B. Port 465)")]
        SslOnConnect = 2
    }
}
