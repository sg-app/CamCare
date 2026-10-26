namespace CamCare.Models
{
    /// <summary>
    /// Ein Bild, das in eine HTML-E-Mail eingebettet wird (Content-Id / <c>cid:</c>) und nicht
    /// über eine externe URL geladen werden muss. E-Mail-Clients blockieren externe Bilder
    /// häufig, deshalb wird das Logo der Signatur eingebettet.
    /// </summary>
    public class MailInlineImage
    {
        /// <summary>Content-Id, über die das Bild im HTML referenziert wird (<c>cid:…</c>).</summary>
        public string ContentId { get; set; } = string.Empty;

        /// <summary>Dateiname des Bildes.</summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>Inhaltstyp (MIME-Typ) des Bildes.</summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>Bilddaten.</summary>
        public byte[] Data { get; set; } = [];
    }
}
