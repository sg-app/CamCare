using Radzen.Blazor;

namespace CamCare.Extensions
{
    public static class RadzenDataGridExtensions
    {
        /// <summary>
        /// Fügt eine neue Zeile am Ende der aktuellen Grid-Ansicht ein (Append) und versetzt sie in
        /// den Bearbeitungsmodus. <see cref="RadzenDataGrid{TItem}.InsertRow(TItem)"/> fügt neue
        /// Zeilen immer an erster Stelle (Index 0) ein – hier wird die Zeile stattdessen hinter die
        /// letzte vorhandene Zeile gehängt.
        /// </summary>
        /// <typeparam name="TItem">Der Typ der Grid-Zeilen.</typeparam>
        /// <param name="grid">Das Grid, in dem die Zeile eingefügt wird.</param>
        /// <param name="item">Die einzufügende Zeile.</param>
        public static async Task AppendRowAsync<TItem>(this RadzenDataGrid<TItem> grid, TItem item)
            where TItem : notnull
        {
            ArgumentNullException.ThrowIfNull(grid);
            ArgumentNullException.ThrowIfNull(item);

            // Anker ist die letzte Zeile der aktuellen Ansicht (inklusive noch nicht gespeicherter
            // Zeilen). InsertAfterRow ermittelt den Einfügeindex in genau dieser Ansicht.
            var lastRow = grid.PagedView.LastOrDefault();

            if (lastRow is null)
                await grid.InsertRow(item);
            else
                await grid.InsertAfterRow(item, lastRow);
        }
    }
}
