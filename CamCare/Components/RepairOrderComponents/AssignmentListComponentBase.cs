using CamCare.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace CamCare.Components.RepairOrderComponents
{
    /// <summary>
    /// Basis für die Zuordnungen eines Reparaturauftrags (Defekte, ausgeführte Arbeiten,
    /// mitgelieferte Komponenten und zu berechnende Artikel).
    /// <para>
    /// Die Einträge werden über ein AutoComplete gesucht bzw. frei erfasst und ans Ende der Liste
    /// gehängt. Ein Katalogtreffer wird bereits bei der Auswahl übernommen (Mausklick oder
    /// Pfeiltasten + Enter); eine Eingabe ohne Auswahl wird mit Enter bzw. über „Einfügen“
    /// angelegt oder – bei exakter Übereinstimmung – als Katalogeintrag übernommen. Beim Einfügen
    /// wird die Reihenfolge mitgespeichert (<c>SortOrder</c>); sie kann anschließend per
    /// Drag&amp;Drop geändert werden.
    /// </para>
    /// </summary>
    /// <typeparam name="TItem">Typ der zugeordneten Einträge (ViewModel).</typeparam>
    public abstract class AssignmentListComponentBase<TItem> : ComponentBase
        where TItem : class
    {
        [Inject] protected ILoggerFactory LoggerFactory { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;

        /// <summary>Auftrag, dem die Einträge zugeordnet werden.</summary>
        [Parameter] public RepairOrderVm RepairOrder { get; set; } = new();

        /// <summary>
        /// Container der Komponente (AutoComplete und Liste). Wird benötigt, um das Eingabefeld
        /// nach dem Übernehmen einer Eingabe zu leeren.
        /// </summary>
        protected ElementReference Container { get; set; }

        /// <summary>Aktueller Inhalt des AutoComplete-Eingabefelds.</summary>
        protected string? SearchText { get; set; }

        /// <summary>Zuletzt übernommener Text (siehe <see cref="OnSearchTextChanged"/>).</summary>
        private string? _insertedText;

        /// <summary>Vom Katalog geladene Auswahleinträge.</summary>
        protected IList<TItem>? Catalog { get; set; }

        private ILogger? _logger;

        /// <summary>Logger der abgeleiteten Komponente.</summary>
        protected ILogger Logger => _logger ??= LoggerFactory.CreateLogger(GetType());

        /// <summary>Sammlung der dem Auftrag zugeordneten Einträge (wird direkt am Auftrag gehalten).</summary>
        protected abstract ICollection<TItem>? Items { get; set; }

        /// <summary>Beschreibungstext eines Eintrags (wird zum Abgleich mit der Eingabe genutzt).</summary>
        protected abstract string GetDescription(TItem item);

        /// <summary>Reihenfolge des Eintrags innerhalb des Auftrags.</summary>
        protected abstract int GetSortOrder(TItem item);

        /// <summary>Setzt die Reihenfolge des Eintrags innerhalb des Auftrags.</summary>
        protected abstract void SetSortOrder(TItem item, int sortOrder);

        /// <summary>Erzeugt einen neuen Eintrag aus einem frei erfassten Text.</summary>
        protected abstract TItem CreateFromText(string text);

        /// <summary>Erzeugt einen dem Auftrag zuzuordnenden Eintrag aus einem Katalogeintrag.</summary>
        protected abstract TItem CreateFromCatalogItem(TItem catalogItem);

        protected override void OnParametersSet()
        {
            if (RepairOrder is null)
                throw new ArgumentNullException(nameof(RepairOrder), "RepairOrder cannot be null");

            // Ohne erfasste Einträge existiert die Sammlung noch nicht.
            Items ??= new List<TItem>();
        }

        /// <summary>
        /// Übernimmt die Eingabe aus dem Eingabefeld. Das change-Event des AutoComplete feuert
        /// beim Enter-Druck (bzw. beim Verlassen des Felds) auch nach dem Einfügen noch nach –
        /// der bereits übernommene Text wird deshalb ignoriert, damit das Feld geleert bleibt.
        /// </summary>
        protected void OnSearchTextChanged(string? value)
        {
            if (_insertedText is not null)
            {
                if (string.Equals(_insertedText, value, StringComparison.Ordinal))
                    return;

                // Es wurde wieder etwas anderes eingegeben – der Merker gilt nicht mehr.
                _insertedText = null;
            }

            SearchText = value;
        }

        /// <summary>
        /// Übernimmt einen im Dropdown ausgewählten Katalogeintrag direkt in die Zuordnung.
        /// Radzen meldet über <c>SelectedItemChanged</c> sowohl die Auswahl mit der Maus als auch
        /// die Auswahl mit den Pfeiltasten und Enter – beide Fälle werden hier eingefügt.
        /// </summary>
        protected async Task OnCatalogItemSelected(object? selected)
        {
            if (selected is not TItem item)
                return;

            Logger.LogDebug("Katalogeintrag '{description}' wurde ausgewählt.", GetDescription(item));

            await InsertCatalogItemAsync(item);
        }

        /// <summary>Fügt die Eingabe mit der Enter-Taste dem Auftrag hinzu.</summary>
        protected async Task OnSearchKeyDown(KeyboardEventArgs args)
        {
            if (args.Key != "Enter")
                return;

            // Ist im Dropdown ein Eintrag markiert (Navigation mit den Pfeiltasten), übernimmt
            // Radzen die Auswahl beim Enter-Druck selbst und ruft `SelectedItemChanged` auf.
            // Ein zusätzliches Einfügen hier würde den Eintrag doppelt anlegen.
            if (await HasHighlightedCatalogItemAsync())
                return;

            // Das change-Event des RadzenAutoComplete (ValueChanged-Modellbindung) feuert bei
            // Enter noch nicht – erst beim Verlassen des Felds (Blur). Damit die Eingabe
            // übernommen wird, wird der aktuelle Wert direkt aus dem Eingabefeld gelesen.
            var value = await JS.InvokeAsync<string?>("camcareGetActiveElementValue");
            var text = value?.Trim();

            if (string.IsNullOrWhiteSpace(text) || string.Equals(text, _insertedText, StringComparison.Ordinal))
                return;

            SearchText = text;
            await InsertAsync();
        }

        /// <summary>
        /// Prüft, ob im AutoComplete-Dropdown ein Eintrag markiert ist (Navigation mit den
        /// Pfeiltasten). Nur dann wählt Radzen den Eintrag beim Enter-Druck selbst aus.
        /// </summary>
        private async Task<bool> HasHighlightedCatalogItemAsync()
        {
            try
            {
                return await JS.InvokeAsync<bool>("camcareHasHighlightedAutoCompleteItem", Container);
            }
            catch (JSDisconnectedException)
            {
                // Der Circuit wurde beendet – es kann nichts mehr markiert sein.
                return false;
            }
            catch (JSException)
            {
                // Das Dropdown ist nicht (mehr) vorhanden – dann gibt es auch keine Markierung.
                return false;
            }
        }

        /// <summary>Fügt die Eingabe über den Button „Einfügen“ dem Auftrag hinzu.</summary>
        protected Task OnInsertClicked(MouseEventArgs args) => InsertAsync();

        /// <summary>Leert das Eingabefeld des AutoComplete (nach dem Übernehmen einer Eingabe).</summary>
        protected async Task ClearSearchInputAsync()
        {
            try
            {
                await JS.InvokeVoidAsync("camcareClearAutoComplete", Container);
            }
            catch (JSDisconnectedException)
            {
                // Der Circuit wurde beendet – das Eingabefeld ist nicht mehr vorhanden.
            }
            catch (JSException ex)
            {
                // Das Eingabefeld ist (noch) nicht gerendert – das Leeren kann entfallen.
                Logger.LogDebug(ex, "Das AutoComplete-Eingabefeld konnte nicht geleert werden.");
            }
        }

        /// <summary>
        /// Hängt den eingegebenen Eintrag ans Ende der Zuordnung an und vergibt die Reihenfolge.
        /// Entspricht die Eingabe exakt einem Katalogeintrag, wird dieser ausgewählt; ansonsten
        /// wird ein neuer Eintrag aus dem erfassten Text angelegt (und beim Speichern des Auftrags
        /// in den Stammdaten hinterlegt).
        /// </summary>
        protected async Task InsertAsync()
        {
            var text = SearchText?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return;

            var catalogItem = Catalog?.FirstOrDefault(item =>
                string.Equals(GetDescription(item), text, StringComparison.OrdinalIgnoreCase));

            if (catalogItem is not null)
                await InsertCatalogItemAsync(catalogItem);
            else
                await InsertItemAsync(CreateFromText(text), text);
        }

        /// <summary>Hängt einen im Dropdown ausgewählten Katalogeintrag ans Ende der Zuordnung an.</summary>
        protected async Task InsertCatalogItemAsync(TItem catalogItem)
            => await InsertItemAsync(CreateFromCatalogItem(catalogItem), GetDescription(catalogItem));

        /// <summary>
        /// Hängt einen Eintrag ans Ende der Zuordnung an. Bereits zugeordnete Einträge werden nicht
        /// doppelt angelegt; in diesem Fall wird lediglich die Suche zurückgesetzt.
        /// </summary>
        private async Task InsertItemAsync(TItem newItem, string description)
        {
            var items = Items ??= new List<TItem>();

            if (items.Any(item => string.Equals(GetDescription(item), description, StringComparison.OrdinalIgnoreCase)))
            {
                Logger.LogDebug("'{description}' ist dem Auftrag bereits zugeordnet.", description);
                await ResetSearchAsync(description);
                return;
            }

            SetSortOrder(newItem, items.Count);
            items.Add(newItem);

            Logger.LogInformation("'{item}' wurde dem Auftrag {repairOrderId} hinzugefügt (Position {sortOrder}).",
                description, RepairOrder.Id, GetSortOrder(newItem));

            await ResetSearchAsync(description);
        }

        /// <summary>
        /// Setzt die Suche zurück, nachdem eine Eingabe übernommen wurde. Der übernommene Text
        /// wird gemerkt, damit das nachlaufende change-Event des AutoComplete ihn nicht erneut in
        /// das Eingabefeld schreibt.
        /// </summary>
        private async Task ResetSearchAsync(string insertedText)
        {
            _insertedText = insertedText;
            SearchText = null;
            await ClearSearchInputAsync();
        }

        /// <summary>Entfernt einen Eintrag aus der Zuordnung und nummeriert die Liste neu.</summary>
        protected void OnRemoveClicked(TItem item)
        {
            if (Items is null || !Items.Remove(item))
                return;

            Logger.LogInformation("'{item}' wurde aus dem Auftrag {repairOrderId} entfernt.",
                GetDescription(item), RepairOrder.Id);

            Renumber();
        }

        /// <summary>Verschiebt einen Eintrag (Drag&amp;Drop) und speichert die neue Reihenfolge.</summary>
        protected void OnMove(AssignmentMoveRequest request)
        {
            if (Items is null)
                return;

            var items = Items.ToList();
            if (request.From < 0 || request.From >= items.Count
                || request.To < 0 || request.To >= items.Count
                || request.From == request.To)
            {
                return;
            }

            var moved = items[request.From];
            items.RemoveAt(request.From);
            items.Insert(request.To, moved);

            Items = items;
            Renumber();

            Logger.LogDebug("'{item}' wurde im Auftrag {repairOrderId} an Position {sortOrder} verschoben.",
                GetDescription(moved), RepairOrder.Id, GetSortOrder(moved));
        }

        /// <summary>Nummeriert die Einträge anhand ihrer Listenposition neu (SortOrder = Position).</summary>
        protected void Renumber()
        {
            if (Items is null)
                return;

            var position = 0;
            foreach (var item in Items)
                SetSortOrder(item, position++);
        }

        /// <summary>Index des Eintrags, der gerade per Drag&amp;Drop gezogen wird.</summary>
        protected int? DragIndex { get; private set; }

        /// <summary>Index, über dem der gezogene Eintrag gerade abgelegt werden würde.</summary>
        protected int? DropIndex { get; private set; }

        protected void OnDragStart(int index)
        {
            DragIndex = index;
            DropIndex = index;
        }

        /// <summary>
        /// Wird während des Ziehens sehr häufig ausgelöst; die Zielzeile wird daher nur bei einer
        /// Änderung neu gesetzt.
        /// </summary>
        protected void OnDragOver(int index)
        {
            if (DragIndex is null || DropIndex == index)
                return;

            DropIndex = index;
        }

        protected void OnDrop(int index)
        {
            if (DragIndex is int from)
                OnMove(new AssignmentMoveRequest(from, index));

            OnDragEnd();
        }

        protected void OnDragEnd()
        {
            DragIndex = null;
            DropIndex = null;
        }

        /// <summary>Zusätzliche CSS-Klassen einer Zeile (Zieh- und Ablege-Markierung).</summary>
        protected string GetRowClass(int index)
        {
            var classes = new List<string>();

            if (DragIndex == index)
                classes.Add("camcare-assignment-dragging");

            if (DragIndex is not null && DropIndex == index && DragIndex != index)
                classes.Add("camcare-assignment-drop-target");

            return string.Join(' ', classes);
        }
    }
}
