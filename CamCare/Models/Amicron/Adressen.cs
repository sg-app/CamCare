using System.Text;

namespace CamCare.Models.Amicron
{
    public class Adressen
    {
        public int LfdNr { get; set; }
        public string? KdNummer { get; set; }
        public string? Art { get; set; }
        public string? Vorname { get; set; }
        public string? Name { get; set; }
        public string? Strasse { get; set; }
        public string? Land { get; set; }
        public string? Plz { get; set; }
        public string? Ort { get; set; }
        public string? Zahlweise { get; set; }

        public ICollection<Serials>? Serials { get; set; }

        public string DisplayName => $"[{KdNummer}] - {Name} - {Plz} {Ort}";
        public string FullName
        {
            get
            {
                var sb = new StringBuilder();
                if(!string.IsNullOrEmpty(Vorname))
                    sb.Append(Vorname);
                if (!string.IsNullOrEmpty(Name))
                    sb.Append(' ');
                    sb.Append(Name);
                return sb.ToString();
            }
        }
    }
}
