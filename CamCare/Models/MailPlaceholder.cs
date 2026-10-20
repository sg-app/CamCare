namespace CamCare.Models
{
    /// <summary>
    /// Ein in E-Mail-Vorlagen verfügbarer Platzhalter (Schreibweise <c>{{Token}}</c>).
    /// </summary>
    public class MailPlaceholder
    {
        /// <summary>Name des Platzhalters ohne Klammern (z. B. "OrderNumber").</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Beschreibung des Inhalts.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Beispielwert zur Anzeige in der Konfigurationsoberfläche.</summary>
        public string SampleValue { get; set; } = string.Empty;

        /// <summary>Der Platzhalter in der Schreibweise für die Vorlage (z. B. "{{OrderNumber}}").</summary>
        public string Template { get => $"{{{{{Token}}}}}"; }
    }
}
