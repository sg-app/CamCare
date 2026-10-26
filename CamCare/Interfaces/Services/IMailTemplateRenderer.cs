using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Ersetzt die Platzhalter (<c>{{Token}}</c>) in E-Mail-Vorlagen durch Werte
    /// eines Reparaturauftrags. Neben einfachen Platzhaltern werden folgende
    /// Funktionen unterstützt:
    /// <list type="bullet">
    ///   <item><description><c>{{Token|Ersatzwert}}</c> – Ersatzwert, wenn der Wert leer ist</description></item>
    ///   <item><description><c>{{#if Token}} … {{else}} … {{/if}}</c> – Inhalt nur bei gefülltem Wert</description></item>
    ///   <item><description><c>{{#unless Token}} … {{/unless}}</c> – Inhalt nur bei leerem Wert</description></item>
    ///   <item><description><c>{{! Kommentar}}</c> – wird nicht ausgegeben</description></item>
    /// </list>
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
