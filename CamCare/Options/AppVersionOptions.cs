namespace CamCare.Options
{
    public class AppVersionOptions
    {
        public const string SectionName = "AppVersion";

        public string FullSemVer { get; set; } = "dev";
    }
}
