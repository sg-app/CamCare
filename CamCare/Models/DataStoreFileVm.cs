namespace CamCare.Models
{
    public class DataStoreFileVm
    {
        public string Filename { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public byte[] Content { get; set; } = [];
    }
}
