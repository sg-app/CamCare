namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Erzeugt und validiert zeitlich begrenzte, HMAC-SHA256-signierte Upload-Tokens
    /// für den mobilen Datei-Upload zu einem Reparaturauftrag.
    /// </summary>
    public interface IUploadTokenService
    {
        /// <summary>
        /// Erzeugt ein Token für den angegebenen Auftrag.
        /// Format: {RepairOrderId}.{AblaufzeitUTC}.{Base64Url-Signatur}
        /// </summary>
        string CreateToken(int repairOrderId);

        /// <summary>
        /// Prüft ein Token (Signatur + Ablaufzeit). Gibt bei Erfolg die RepairOrderId zurück.
        /// </summary>
        bool TryValidate(string token, out int repairOrderId);
    }
}
