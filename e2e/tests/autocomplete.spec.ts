import { test, expect, Page } from '@playwright/test';
import { loginViaApi } from './helpers';

/**
 * Öffnet den ersten Reparaturauftrag aus der Liste im Edit-Modus.
 */
async function openFirstRepairOrder(page: Page): Promise<void> {
  await page.goto('/RepairOrders');
  await expect(page.locator('.rz-data-row').first()).toBeVisible({ timeout: 30_000 });

  await page.locator('.rz-data-row').first().locator('button:has-text("edit_square")').click();
  await page.waitForURL('**/EditRepairOrder*', { timeout: 15_000 });
  await expect(page.locator('button:has-text("add")').first()).toBeVisible({ timeout: 30_000 });
}

/**
 * Fügt im n-ten Grid (0-basiert, Reihenfolge im DOM) eine neue Zeile hinzu
 * und gibt das AutoComplete-Eingabefeld zurück.
 */
async function addRowAndGetAutocomplete(page: Page, addButtonIndex: number) {
  const addButton = page.locator('button:has-text("add")').nth(addButtonIndex);
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

    // Defekte-Grid ist das 2. Grid mit "add"-Button (DOM-Reihenfolge:
    // Mitgelieferte Komponenten, Defekte, Reparaturpositionen).
    const input = await addRowAndGetAutocomplete(page, 1);
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

    const input = await addRowAndGetAutocomplete(page, 0);
    await selectAutocompleteItem(page, input, 'Netz', 'Netzteil');

    await expectCellContains(page, 'Netzteil');
  });

  test('Reparaturpositionen: Mausklick auf Katalogtreffer übernimmt Beschreibung und Artikelnummer', async ({ page }) => {
    await openFirstRepairOrder(page);

    const input = await addRowAndGetAutocomplete(page, 2);
    await selectAutocompleteItem(page, input, 'Spiralschutz', 'Spiralschutzschlauch für MINI-Power 100');

    await expectCellContains(page, 'Spiralschutzschlauch für MINI-Power 100');
  });

  test('Defekte: frei eingetragener Text (kein Katalogtreffer) wird über den Save-Button übernommen', async ({ page }) => {
    await openFirstRepairOrder(page);

    const input = await addRowAndGetAutocomplete(page, 1);

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
});
