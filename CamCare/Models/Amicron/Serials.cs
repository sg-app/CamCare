namespace CamCare.Models.Amicron
{
    public class Serials
    {
        public int LfdNr { get; set; }
        public int ArtikelLfdNr { get; set; }
        public string? Seriennummer { get; set; }
        public int? KundenLfdNr { get; set; }
        public string? Artikelbezeichnung { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerPlz { get; set; }

        /// <summary>
        /// True, wenn für diese Seriennummer aktuell ein offener (nicht abgeschlossener)
        /// Reparaturauftrag existiert. Wird in den Listenansichten zur Anzeige eines Icons verwendet.
        /// </summary>
        public bool HasOpenOrder { get; set; }

        public string DisplayName => $"{Seriennummer} - {Artikelbezeichnung}";
    }
}
