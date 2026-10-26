namespace CamCare.Models
{
    /// <summary>
    /// Das fertige HTML der E-Mail-Signatur sowie das darin referenzierte Logo.
    /// </summary>
    public class MailSignatureContent
    {
        /// <summary>HTML der Signatur; der Logo-Platzhalter ist bereits ersetzt.</summary>
        public string Html { get; init; } = string.Empty;

        /// <summary>Das in die E-Mail einzubettende Logo oder <c>null</c>.</summary>
        public MailInlineImage? Logo { get; init; }
    }
}
