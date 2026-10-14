namespace CamCare.Models;
/// <summary>
/// Antwort der Auth-Endpunkte: Ziel-URL für den Full-Reload nach Login/Logout.
/// </summary>
internal sealed class AuthResult
{
    public string Redirect { get; set; } = "/";
}
