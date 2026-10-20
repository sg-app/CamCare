namespace CamCare.Models.Amicron
{
    public class AdressenFilter
    {
        public string? KdNummer { get; set; }
        public string? Name { get; set; }
        public string? Plz { get; set; }
        public string? Ort { get; set; }

        /// <summary>
        /// Freitextsuche, die gleichzeitig auf Kd-Nummer, Vorname, Name, PLZ und Ort angewendet
        /// wird (ODER-verknüpft). Ermöglicht eine Kundensuche über eine einzige Suchzeile.
        /// </summary>
        public string? SearchText { get; set; }
    }
}
