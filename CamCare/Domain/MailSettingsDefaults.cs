namespace CamCare.Domain
{
    /// <summary>
    /// Standardwerte für die E-Mail-Konfiguration (Ausgangsvorlagen für neue Installationen).
    /// </summary>
    public static class MailSettingsDefaults
    {
        /// <summary>Vorlage für den Status "Eingetroffen" (Standard: Status-Id 2).</summary>
        public const string ArrivedSubjectTemplate = "Ihr Gerät ist eingetroffen – Auftrag {{OrderNumber}}";

        public const string ArrivedBodyTemplate = """
            <p>Guten Tag {{CustomerName}},</p>
            <p>Ihr Gerät (Seriennummer {{SerialNumber}}) ist zu Ihrem Reparaturauftrag
            <strong>{{OrderNumber}}</strong> bei uns eingetroffen.</p>
            <p>Eingetroffen am: {{ArrivedAt}}</p>
            <p>Wir beginnen mit der Begutachtung und melden uns mit den nächsten Schritten.</p>
            <p>Mit freundlichen Grüßen<br />Ihr CamCare-Team</p>
            """;

        /// <summary>Vorlage für den Status "Versendet" (Standard: Status-Id 9).</summary>
        public const string ShippedSubjectTemplate = "Ihr Auftrag {{OrderNumber}} wurde versendet – {{Status}}";

        public const string ShippedBodyTemplate = """
            <p>Guten Tag {{CustomerName}},</p>
            <p>Ihr Reparaturauftrag <strong>{{OrderNumber}}</strong> (Seriennummer {{SerialNumber}})
            wurde an Sie versendet.</p>
            <p>Lieferanschrift:<br />{{DeliveryAddressRecipient}}<br />{{DeliveryAddressStreet}}<br />{{DeliveryAddressCity}}</p>
            <p>Stand: {{CurrentStateAt}}</p>
            <p>Mit freundlichen Grüßen<br />Ihr CamCare-Team</p>
            """;

        /// <summary>
        /// Platzhalter in der Signatur, an dem das Logo der Signatur ausgegeben wird.
        /// </summary>
        public const string SignatureLogoPlaceholder = "{{Logo}}";

        /// <summary>Standardinhalt einer neu angelegten Signatur.</summary>
        public const string SignatureHtmlTemplate = """
            <p>Mit freundlichen Grüßen</p>
            <p><strong>Ihr CamCare-Team</strong></p>
            <p>{{Logo}}</p>
            """;

        /// <summary>Allgemeine Vorlage für neu angelegte Vorlagen.</summary>
        public const string SubjectTemplate = "Ihr Reparaturauftrag {{OrderNumber}} – {{Status}}";

        public const string BodyTemplate = """
            <p>Guten Tag {{CustomerName}},</p>
            <p>der Status Ihres Reparaturauftrags <strong>{{OrderNumber}}</strong>
            (Seriennummer {{SerialNumber}}) hat sich geändert:</p>
            <p><strong>{{Status}}</strong> – Stand: {{CurrentStateAt}}</p>
            <p>Bei Fragen antworten Sie einfach auf diese E-Mail.</p>
            <p>Mit freundlichen Grüßen<br />Ihr CamCare-Team</p>
            """;
    }
}
