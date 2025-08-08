using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class Parameter
    {
        [Key]
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

    }
}
