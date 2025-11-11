namespace CamCare.Models.Amicron
{
    public class Artikel
    {
        public int LfdNr { get; set; }
        public string? Artikelnummer { get; set; }
        public string? Description { get; set; }
        public decimal? InStock { get; set; }
        public decimal? MinStock { get; set; }
        public string? Unit { get; set; }
        public byte[]? Picture { get; set; }
    }
}
