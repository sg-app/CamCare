namespace CamCare.Domain
{
    /// <summary>
    /// Ermittelt den Inhaltstyp (MIME-Typ) eines Bildes anhand der Dateiendung.
    /// Wird für das Logo der E-Mail-Signatur benötigt.
    /// </summary>
    public static class ImageContentTypes
    {
        /// <summary>Inhaltstyp, wenn die Dateiendung unbekannt ist.</summary>
        public const string DefaultContentType = "image/png";

        /// <summary>Liefert den Inhaltstyp zu einem Dateinamen (z. B. <c>logo.png</c> → <c>image/png</c>).</summary>
        public static string FromFileName(string? fileName) => Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" or ".jpe" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".ico" => "image/x-icon",
            _ => DefaultContentType
        };
    }
}
