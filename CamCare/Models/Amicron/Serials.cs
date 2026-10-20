namespace CamCare.Models.Amicron
{
    public class Serials
    {
        public int LfdNr { get; set; }
        public int ArtikelLfdNr { get; set; }
        public string? Seriennummer { get; set; }
        public int? KundenLfdNr { get; set; }
        public string? Artikelnumber { get; set; }
        public string? Artikelbezeichnung { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerPlz { get; set; }

        /// <summary>
        /// True, wenn für diese Seriennummer aktuell ein offener (nicht abgeschlossener)
        /// Reparaturauftrag existiert. Wird in den Listenansichten zur Anzeige eines Icons verwendet.
        /// </summary>
        public bool HasOpenOrder { get; set; }

        public string DisplayName => $"{Seriennummer} - {Artikelnumber} - {Artikelbezeichnung}";

        /// <summary>
        /// Anzeigename für die Seriennummern-Auswahl im Reparaturauftrag. Fehlende Bestandteile
        /// (z. B. bei Bestandsdaten ohne gepflegte Artikelnummer/Bezeichnung) werden ausgelassen,
        /// damit keine leeren " - " entstehen.
        /// </summary>
        public string SelectionDisplayName => string.Join(" - ",
            new[] { Seriennummer, Artikelnumber, Artikelbezeichnung }
                .Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}
