namespace CamCare.Models;
/// <summary>
/// JSON-Antwort des Mobilen-Upload-Endpunkts (POST /api/upload/{token}).
/// </summary>
internal sealed class UploadResult
{
    public bool Success { get; set; }

    /// <summary>Erfolgreich gespeicherte Dateinamen.</summary>
    public List<string> Files { get; set; } = [];

    /// <summary>Fehlermeldungen pro Datei (leer, wenn alles erfolgreich war).</summary>
    public List<string> Errors { get; set; } = [];
}
