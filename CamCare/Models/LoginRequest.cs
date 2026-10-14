namespace CamCare.Models;
/// <summary>
/// Anmelde-Daten des Login-Dialogs (POST /api/auth/login, JSON).
/// </summary>
internal sealed class LoginRequest
{
    public string Group { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
