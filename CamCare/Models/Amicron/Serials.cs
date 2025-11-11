namespace CamCare.Models.Amicron
{
    public class Serials
    {
        public int LfdNr { get; set; }
        public int ArtikelLfdNr{ get; set; }
        public string? Seriennummer { get; set; }
        public int? KundenLfdNr { get; set; }
        public string? Artikelbezeichnung { get; set; }

        public string DisplayName => $"{Seriennummer} - {Artikelbezeichnung}";
    }
}
