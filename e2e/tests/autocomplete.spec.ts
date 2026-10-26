import { test, expect, Page } from '@playwright/test';
import { loginViaApi } from './helpers';

/**
 * Öffnet den ersten Reparaturauftrag aus der Liste im Edit-Modus.
 */
async function openFirstRepairOrder(page: Page): Promise<void> {
  await page.goto('/RepairOrders');
  await expect(page.locator('.rz-data-row').first()).toBeVisible({ timeout: 30_000 });

  await page.locator('.rz-data-row').first().locator('button:has-text("edit_square")').click();

  // Blazor navigiert clientseitig (Enhanced Navigation); dabei wird kein Load-Event ausgelöst.
  // Deshalb wird die URL geprüft und auf die Edit-Seite gewartet – nicht auf `load`.
  await expect(page).toHaveURL(/\/EditRepairOrder/, { timeout: 15_000 });
  await expect(page.locator('button').filter({ hasText: /^\s*add\s*$/ }).first())
    .toBeVisible({ timeout: 30_000 });
}

/**
 * Liefert den "Einfügen"-Button des Grids mit der angegebenen Überschrift
 * (z. B. "Defekte", "Mitgelieferte Komponenten", "Zu berechnende Artikel").
 *
 * Hinweis: `button:has-text("add")` ist zu unspezifisch – es matcht auch die
 * Icon-Buttons "add_location_alt" (Lieferanschrift) und "add_comment" (E-Mail).
 */
function addButtonForGrid(page: Page, gridTitle: string) {
  return page
    .locator('div')
    .filter({ has: page.locator('h6', { hasText: new RegExp(`^\\s*${gridTitle}\\s*$`) }) })
    .last()
    .locator('button')
    .filter({ hasText: /^\s*add\s*$/ });
}

/**
 * Fügt im Grid mit der angegebenen Überschrift eine neue Zeile hinzu
 * und gibt das AutoComplete-Eingabefeld zurück.
 */
async function addRowAndGetAutocomplete(page: Page, gridTitle: string) {
  const addButton = addButtonForGrid(page, gridTitle).first();
  await expect(addButton).toBeVisible({ timeout: 30_000 });
  await addButton.click();

  const input = page.locator('input.rz-autocomplete-input').first();
  await expect(input).toBeVisible({ timeout: 30_000 });
  return input;
}

/**
 * Liest alle sichtbaren Tabellenzellen aller RadzenDataGrids aus.
 */
async function getAllGridCellTexts(page: Page): Promise<string[][]> {
  return page.evaluate(() => {
    const tables = Array.from(document.querySelectorAll('.rz-datatable'));
    return tables.map((t) =>
      Array.from(t.querySelectorAll('.rz-data-row')).map((r) =>
        (r.textContent ?? '').replace(/\s+/g, ' ').trim()));
  });
}

/**
 * Liest die Zeilen (als Text) des Grids, das den gesuchten Text enthält.
 */
async function getRowsOfGridContaining(page: Page, text: string): Promise<string[]> {
  return page.evaluate((needle) => {
    const tables = Array.from(document.querySelectorAll('.rz-datatable'));
    for (const table of tables) {
      const rows = Array.from(table.querySelectorAll('.rz-data-row')).map((r) =>
        (r.textContent ?? '').replace(/\s+/g, ' ').trim());
      if (rows.some((row) => row.includes(needle))) {
        return rows;
      }
    }
    return [];
  }, text);
}

async function expectCellContains(page: Page, text: string): Promise<void> {
  await expect
    .poll(async () => {
      const grids = await getAllGridCellTexts(page);
      return grids.some((rows) => rows.some((row) => row.includes(text)));
    }, { timeout: 15_000 })
    .toBe(true);
}

/**
 * Simuliert das vom Tester gemeldete Szenario: Zeichen eingeben, warten bis
 * die gefilterten Elemente erscheinen, dann per Mausklick ein Element wählen.
 */
async function selectAutocompleteItem(
  page: Page,
  input: ReturnType<typeof page.locator>,
  searchText: string,
  expectedItemText: string,
) {
  await input.click();
  await input.pressSequentially(searchText, { delay: 100 });

  const item = page.locator('.rz-autocomplete-list-item', { hasText: expectedItemText }).first();
  await expect(item).toBeVisible({ timeout: 15_000 });

  await item.click();
}

test.describe('RadzenAutoComplete Mausklick-Übernahme', () => {
  test.beforeEach(async ({ page }) => {
    await loginViaApi(page, 'buero', 'buero');
  });

  test('Defekte: Mausklick auf Katalogtreffer übernimmt den vollen Wert', async ({ page }) => {
    await openFirstRepairOrder(page);

    const input = await addRowAndGetAutocomplete(page, 'Defekte');
    await selectAutocompleteItem(page, input, 'Glas', 'Glas gebrochen');

    // Erwartung: Die Zelle zeigt den vollständigen Katalogtext, nicht nur den getippten Teil.
    await expectCellContains(page, 'Glas gebrochen');
    // Regression: Es darf keine Zelle mit dem unvollständigen getippten Wert ("Glas") übrig bleiben.
    await expect
      .poll(async () => {
        const grids = await getAllGridCellTexts(page);
        const rows = grids.flat().filter((r) => r.includes('Glas'));
        return rows.some((r) => !r.includes('Glas gebrochen') && r.trim() !== 'Glas');
      }, { timeout: 5_000 })
      .toBe(false);
  });

  test('Mitgelieferte Komponenten: Mausklick auf Katalogtreffer übernimmt den vollen Wert', async ({ page }) => {
    await openFirstRepairOrder(page);

    const input = await addRowAndGetAutocomplete(page, 'Mitgelieferte Komponenten');
    await selectAutocompleteItem(page, input, 'Netz', 'Netzteil');

    await expectCellContains(page, 'Netzteil');
  });

  test('Reparaturpositionen: Mausklick auf Katalogtreffer übernimmt Beschreibung und Artikelnummer', async ({ page }) => {
    await openFirstRepairOrder(page);

    const input = await addRowAndGetAutocomplete(page, 'Zu berechnende Artikel');
    await selectAutocompleteItem(page, input, 'Spiralschutz', 'Spiralschutzschlauch für MINI-Power 100');

    await expectCellContains(page, 'Spiralschutzschlauch für MINI-Power 100');
  });

  test('Defekte: frei eingetragener Text (kein Katalogtreffer) wird über den Save-Button übernommen', async ({ page }) => {
    await openFirstRepairOrder(page);

    const input = await addRowAndGetAutocomplete(page, 'Defekte');

    // Freier Text, der nicht im Katalog existiert.
    const freeText = 'Defekt Sonderanfertigung';
    await input.click();
    await input.pressSequentially(freeText);

    // Neue Zeile über den Save-Haken des Inline-Edits übernehmen.
    const saveButton = page.locator('button:has-text("save")').first();
    await expect(saveButton).toBeVisible({ timeout: 15_000 });
    await saveButton.click();

    await expectCellContains(page, freeText);
  });

  test('Defekte: neue Zeilen werden am Ende der Liste angehängt (Append statt Index 0)', async ({ page }) => {
    await openFirstRepairOrder(page);

    // Erste neue Zeile über einen Katalogtreffer (Mausklick).
    const firstInput = await addRowAndGetAutocomplete(page, 'Defekte');
    await selectAutocompleteItem(page, firstInput, 'Glas', 'Glas gebrochen');
    await expectCellContains(page, 'Glas gebrochen');

    // Zweite neue Zeile mit freiem Text (kein Katalogtreffer), gespeichert über den Save-Haken.
    const secondInput = await addRowAndGetAutocomplete(page, 'Defekte');
    const freeText = 'Defekt Append-Test';
    await secondInput.click();
    await secondInput.pressSequentially(freeText);

    const saveButton = page.locator('button:has-text("save")').first();
    await expect(saveButton).toBeVisible({ timeout: 15_000 });
    await saveButton.click();
    await expectCellContains(page, freeText);

    // Erwartung: Die zuletzt hinzugefügte Zeile steht UNTER der zuvor hinzugefügten Zeile –
    // neue Zeilen werden also ans Ende gehängt und nicht an erster Stelle eingefügt.
    await expect
      .poll(async () => {
        const rows = await getRowsOfGridContaining(page, freeText);
        const firstIndex = rows.findIndex((row) => row.includes('Glas gebrochen'));
        const secondIndex = rows.findIndex((row) => row.includes(freeText));
        return firstIndex >= 0 && secondIndex > firstIndex;
      }, { timeout: 15_000 })
      .toBe(true);
  });
});
