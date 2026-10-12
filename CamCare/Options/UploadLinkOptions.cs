namespace CamCare.Options
{
    /// <summary>
    /// Konfiguration für den mobilen Datei-Upload per Link/QR-Code (anonyme Upload-Seite).
    /// </summary>
    public class UploadLinkOptions
    {
        public const string SectionName = "UploadLink";

        /// <summary>
        /// Geheimer Schlüssel zur HMAC-SHA256-Signierung der Upload-Tokens.
        /// Muss in Produktion über eine sichere Umgebungsvariable gesetzt werden.
        /// </summary>
        public string Secret { get; set; } = string.Empty;

        /// <summary>
        /// Gültigkeitsdauer eines Upload-Links in Minuten.
        /// </summary>
        public int TokenLifetimeMinutes { get; set; } = 120;

        /// <summary>
        /// Öffentliche Basis-URL unter der die Upload-Seite erreichbar ist
        /// (z. B. https://upload.firma.de). Wird für QR-Code und Link verwendet.
        /// </summary>
        public string PublicBaseUrl { get; set; } = string.Empty;
    }
}
