using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Ersetzt die Platzhalter (<c>{{Token}}</c>) in E-Mail-Vorlagen durch Werte
    /// eines Reparaturauftrags.
    /// </summary>
    public interface IMailTemplateRenderer
    {
        /// <summary>
        /// Liefert alle verfügbaren Platzhalter mit Beschreibung und Beispielwert
        /// (für die Anzeige in der Konfigurationsoberfläche).
        /// </summary>
        IReadOnlyList<MailPlaceholder> GetPlaceholders();

        /// <summary>
        /// Ersetzt alle bekannten Platzhalter der Vorlage durch die Werte des Auftrags.
        /// Unbekannte Platzhalter bleiben unverändert stehen.
        /// </summary>
        string Render(string? template, RepairOrderVm? order);
    }
}
