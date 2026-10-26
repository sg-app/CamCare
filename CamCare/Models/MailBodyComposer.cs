namespace CamCare.Models
{
    /// <summary>
    /// Fügt den Inhalt einer E-Mail und die Signatur zu einem vollständigen HTML-Text zusammen.
    /// </summary>
    public static class MailBodyComposer
    {
        /// <summary>
        /// Stil der Signatur. E-Mail-Clients werten in der Regel keine CSS-Klassen aus, deshalb
        /// werden die Formatierungen als Inline-Styles gesetzt.
        /// </summary>
        private const string SignatureWrapperStyle =
            "margin-top:16px;padding-top:12px;border-top:1px solid #e0e0e0;"
            + "color:#333333;font-family:Arial,Helvetica,sans-serif;font-size:13px;";

        /// <summary>
        /// Hängt die Signatur an den E-Mail-Inhalt an. Ist keine Signatur vorhanden, wird der
        /// Inhalt unverändert zurückgegeben.
        /// </summary>
        public static string AppendSignature(string? body, string? signatureHtml)
            => string.IsNullOrWhiteSpace(signatureHtml)
                ? body ?? string.Empty
                : $"{body}<br /><div style=\"{SignatureWrapperStyle}\">{signatureHtml}</div>";
    }
}
