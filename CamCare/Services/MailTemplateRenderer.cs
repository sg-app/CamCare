using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using CamCare.Interfaces.Services;
using CamCare.Models;

namespace CamCare.Services
{
    /// <summary>
    /// Ersetzt die Platzhalter (<c>{{Token}}</c>) in E-Mail-Vorlagen durch Werte eines
    /// Reparaturauftrags. Die Werte werden HTML-kodiert, damit Auftragsdaten die
    /// HTML-Struktur der Vorlage nicht beschädigen.
    /// </summary>
    public partial class MailTemplateRenderer : IMailTemplateRenderer
    {
        private const string DateFormat = "dd.MM.yyyy";
        private const string DateTimeFormat = "dd.MM.yyyy HH:mm";

        /// <summary>Alle unterstützten Platzhalter inkl. Beispielwert.</summary>
        private static readonly IReadOnlyList<MailPlaceholder> Placeholders =
        [
            new() { Token = "OrderId", Description = "Interne Auftrags-Id", SampleValue = "42" },
            new() { Token = "OrderNumber", Description = "Auftragsnummer", SampleValue = "A-2026-00042" },
            new() { Token = "SerialNumber", Description = "Hauptseriennummer", SampleValue = "9876543210" },
            new() { Token = "Status", Description = "Aktueller Status des Auftrags", SampleValue = "Versendet" },
            new() { Token = "CustomerNumber", Description = "Kundennummer", SampleValue = "10042" },
            new() { Token = "CustomerName", Description = "Name des Kunden", SampleValue = "Max Mustermann" },
            new() { Token = "CustomerStreet", Description = "Straße des Kunden", SampleValue = "Musterstraße 1" },
            new() { Token = "CustomerPostalCode", Description = "Postleitzahl des Kunden", SampleValue = "12345" },
            new() { Token = "CustomerCity", Description = "Ort des Kunden", SampleValue = "Musterstadt" },
            new() { Token = "QuoteNumber", Description = "Angebotsnummer", SampleValue = "AN-2026-00042" },
            new() { Token = "DeliveryNoteNumber", Description = "Lieferscheinnummer", SampleValue = "LS-2026-00042" },
            new() { Token = "Defects", Description = "Erfasste Defekte", SampleValue = "Beispieldefekt" },
            new() { Token = "Works", Description = "Erfasste Arbeiten", SampleValue = "Beispielarbeit" },
            new() { Token = "IncludedComponents", Description = "Mitgelieferte Komponenten", SampleValue = "Beispielkomponente" },
            new() { Token = "RepairPositions", Description = "Materialpositionen", SampleValue = "Beispielmaterial" },
            new() { Token = "Employees", Description = "Zugeordnete Mitarbeiter", SampleValue = "Erika Musterfrau" },
            new() { Token = "DeliveryAddressRecipient", Description = "Empfänger der Lieferanschrift", SampleValue = "Max Mustermann" },
            new() { Token = "DeliveryAddressStreet", Description = "Straße der Lieferanschrift", SampleValue = "Musterstraße 1" },
            new() { Token = "DeliveryAddressCity", Description = "PLZ und Ort der Lieferanschrift", SampleValue = "12345 Musterstadt" },
            new() { Token = "PackagingComment", Description = "Kommentar zur Verpackung", SampleValue = "Kundenkarton" },
            new() { Token = "Warranty", Description = "Garantiefall (Ja/Nein)", SampleValue = "Nein" },
            new() { Token = "PriorityRepair", Description = "Eilreparatur (Ja/Nein)", SampleValue = "Nein" },
            new() { Token = "CreatedAt", Description = "Zeitpunkt der Auftragserfassung", SampleValue = "01.01.2026 09:00" },
            new() { Token = "CurrentStateAt", Description = "Zeitpunkt der letzten Statusänderung", SampleValue = "02.01.2026 14:30" },
            new() { Token = "ArrivedAt", Description = "Zeitpunkt des Eintreffens", SampleValue = "02.01.2026 08:15" },
            new() { Token = "Today", Description = "Aktuelles Datum", SampleValue = "02.01.2026" }
        ];

        public IReadOnlyList<MailPlaceholder> GetPlaceholders() => Placeholders;

        public string Render(string? template, RepairOrderVm? order)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            var values = BuildValues(order);

            return PlaceholderRegex().Replace(template, match =>
            {
                var token = match.Groups["token"].Value;
                if (!values.TryGetValue(token, out var value))
                {
                    // Unbekannte Platzhalter bleiben stehen, damit Tippfehler auffallen.
                    return match.Value;
                }

                return WebUtility.HtmlEncode(value);
            });
        }

        private static Dictionary<string, string> BuildValues(RepairOrderVm? order)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Today"] = FormatDate(DateTime.Now)
            };

            if (order is null)
                return values;

            var customer = order.Customer;
            var deliveryAddress = order.DeliveryAddress;

            values["OrderId"] = order.Id.ToString(CultureInfo.InvariantCulture);
            values["OrderNumber"] = order.OrderNumber ?? string.Empty;
            values["SerialNumber"] = order.CameraSerialNumber ?? string.Empty;
            values["Status"] = order.RepairOrderStatus?.Name ?? string.Empty;
            values["CustomerNumber"] = customer?.KdNummer ?? order.CustomerId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            values["CustomerName"] = customer?.FullName ?? string.Empty;
            values["CustomerStreet"] = customer?.Strasse ?? string.Empty;
            values["CustomerPostalCode"] = customer?.Plz ?? string.Empty;
            values["CustomerCity"] = customer?.Ort ?? string.Empty;
            values["QuoteNumber"] = order.QuoteNumber ?? string.Empty;
            values["DeliveryNoteNumber"] = order.DeliveryNoteNumber ?? string.Empty;
            values["Defects"] = JoinDescriptions(order.Defectives?.Select(d => d.Description));
            values["Works"] = JoinDescriptions(order.Works?.Select(w => w.Description));
            values["IncludedComponents"] = JoinDescriptions(order.IncludedComponents?.Select(c => c.Description));
            values["RepairPositions"] = JoinDescriptions(order.RepairPositions?.Select(p => p.Description));
            values["Employees"] = JoinDescriptions(order.Employees?.Select(e => $"{e.FirstName} {e.LastName}".Trim()));
            values["DeliveryAddressRecipient"] = deliveryAddress?.Recipient ?? string.Empty;
            values["DeliveryAddressStreet"] = deliveryAddress?.Street ?? string.Empty;
            values["DeliveryAddressCity"] = JoinParts(deliveryAddress?.PostalCode, deliveryAddress?.City);
            values["PackagingComment"] = order.PackagingComment ?? string.Empty;
            values["Warranty"] = ToYesNo(order.Warranty);
            values["PriorityRepair"] = ToYesNo(order.PriorityRepair == true);
            values["CreatedAt"] = FormatDateTime(order.CreatedAt);
            values["CurrentStateAt"] = FormatDateTime(order.CurrentStateAt);
            values["ArrivedAt"] = FormatDateTime(order.ArrivedAt);

            return values;
        }

        private static string JoinDescriptions(IEnumerable<string?>? descriptions)
            => descriptions is null
                ? string.Empty
                : JoinParts(descriptions.Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => d!.Trim()).ToArray());

        private static string JoinParts(params string?[] parts)
            => string.Join("; ", parts.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p!.Trim()));

        private static string ToYesNo(bool value) => value ? "Ja" : "Nein";

        private static string FormatDate(DateTime value) => value.ToString(DateFormat, CultureInfo.InvariantCulture);

        private static string FormatDateTime(DateTime? value)
            => value.HasValue ? value.Value.ToString(DateTimeFormat, CultureInfo.InvariantCulture) : string.Empty;

        [GeneratedRegex(@"\{\{\s*(?<token>[A-Za-z0-9_]+)\s*\}\}")]
        private static partial Regex PlaceholderRegex();
    }
}
