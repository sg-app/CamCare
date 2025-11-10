namespace CamCare.Models.Amicron
{
    public class Adressen
    {
        public int LfdNr { get; set; }
        public string? KdNummer { get; set; }
        public string? Art { get; set; }
        public string? Suchbegriff { get; set; }
        public string? Vorname { get; set; }
        public string? Name { get; set; }
        public string? Strasse { get; set; }
        public string? Land { get; set; }
        public string? Plz { get; set; }
        public string? Ort { get; set; }
        public string? Zahlweise { get; set; }

        public string DisplayName => $"[{KdNummer}] - {Name}";
    }
}
