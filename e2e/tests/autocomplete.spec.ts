import { test, expect, Page, Locator } from '@playwright/test';
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
  await expect(assignment(page, 'Defekte')).toBeVisible({ timeout: 30_000 });
}

/* ---------------------------------------------------------------------------------------
 * Zuordnungen mit AutoComplete (Defekte, ausgeführte Arbeiten)
 * ------------------------------------------------------------------------------------ */

/**
 * Liefert den Bereich einer Zuordnung (AutoComplete + Liste der Einträge).
 * Die Bereiche sind über `data-assignment` gekennzeichnet, z. B. "Defekte" und "Arbeiten".
 */
function assignment(page: Page, name: string): Locator {
  return page.locator(`[data-assignment="${name}"]`).first();
}

/** AutoComplete-Eingabefeld einer Zuordnung. */
function searchInput(page: Page, name: string): Locator {
  return assignment(page, name).locator('input.rz-autocomplete-input').first();
}

/** Zeilen (zugeordnete Einträge) einer Zuordnung in der angezeigten Reihenfolge. */
function rows(page: Page, name: string): Locator {
  return assignment(page, name).locator('.camcare-assignment-row');
}

/** Liest die Texte der zugeordneten Einträge in der angezeigten Reihenfolge. */
async function rowTexts(page: Page, name: string): Promise<string[]> {
  return rows(page, name).evaluateAll((elements) =>
    elements.map((element) => (element.textContent ?? '').replace(/\s+/g, ' ').trim()));
}

/**
 * Sucht einen Katalogeintrag und übernimmt den vollständigen Text per Mausklick in das
 * Eingabefeld. Eingefügt wird der Eintrag anschließend mit Enter oder „Einfügen“.
 */
async function selectCatalogItem(page: Page, name: string, searchText: string, itemText: string): Promise<void> {
  const input = searchInput(page, name);
  await input.click();
  await input.pressSequentially(searchText, { delay: 100 });

  const item = page.locator('.rz-autocomplete-list-item', { hasText: itemText }).first();
  await expect(item).toBeVisible({ timeout: 15_000 });
  await item.click();

  // Der vollständige Text des Katalogtreffers wird erst mit dem Server-Roundtrip in das
  // Eingabefeld übernommen – darauf warten, bevor mit Enter eingefügt wird.
  await expect(input).toHaveValue(itemText, { timeout: 15_000 });
}

/** Fügt einen Text über das AutoComplete und die Enter-Taste dem Auftrag hinzu. */
async function insertWithEnter(page: Page, name: string, text: string): Promise<void> {
  const input = searchInput(page, name);
  await input.click();
  await input.pressSequentially(text, { delay: 100 });
  await input.press('Enter');

  await expect(rows(page, name).filter({ hasText: text }).first()).toBeVisible({ timeout: 15_000 });
}

/** Fügt den aktuell im Eingabefeld stehenden Text über den Button „Einfügen“ hinzu. */
async function insertWithButton(page: Page, name: string, text: string): Promise<void> {
  const input = searchInput(page, name);
  await input.click();
  await input.pressSequentially(text, { delay: 100 });

  await assignment(page, name).locator('button').filter({ hasText: /^\s*add\s*$/ }).first().click();

  await expect(rows(page, name).filter({ hasText: text }).first()).toBeVisible({ timeout: 15_000 });
}


/* ---------------------------------------------------------------------------------------
 * Zuordnungen über das Grid (mitgelieferte Komponenten, zu berechnende Artikel)
 * ------------------------------------------------------------------------------------ */

/**
 * Liefert den "Einfügen"-Button des Grids mit der angegebenen Überschrift
 * (z. B. "Mitgelieferte Komponenten", "Zu berechnende Artikel").
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
 * und gibt das AutoComplete-Eingabefeld der neuen Zeile zurück.
 */
async function addRowAndGetAutocomplete(page: Page, gridTitle: string) {
  const addButton = addButtonForGrid(page, gridTitle).first();
  await expect(addButton).toBeVisible({ timeout: 30_000 });
  await addButton.click();

  // Bewusst auf das Grid eingegrenzt: die AutoComplete-Felder der Defekte-/Arbeiten-Zuordnung
  // stehen ebenfalls auf der Seite.
  const input = page.locator('.rz-datatable', { hasText: gridTitle }).locator('input.rz-autocomplete-input').first();
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

test.describe('Zuordnungen über das Grid (Komponenten und Reparaturpositionen)', () => {
  test.beforeEach(async ({ page }) => {
    await loginViaApi(page, 'buero', 'buero');
  });

  test('Mitgelieferte Komponenten: Mausklick auf Katalogtreffer übernimmt den vollen Wert', async ({ page }) => {
    await openFirstRepairOrder(page);

    const input = await addRowAndGetAutocomplete(page, 'Mitgelieferte Komponenten');
    await selectAutocompleteItem(page, input, 'Netz', 'Netzkabel');

    await expectCellContains(page, 'Netzkabel');
  });

  test('Reparaturpositionen: Mausklick auf Katalogtreffer übernimmt die Beschreibung', async ({ page }) => {
    await openFirstRepairOrder(page);

    const input = await addRowAndGetAutocomplete(page, 'Zu berechnende Artikel');
    await selectAutocompleteItem(page, input, 'Schutzglas', 'Schutzglas komplett für 5030');

    // Übernommen wird die vollständige Katalogbeschreibung (nicht der getippte Teil).
    await expectCellContains(page, 'Schutzglas komplett für 5030');
  });
});

test.describe('Zuordnungen über AutoComplete (Defekte und Arbeiten)', () => {
  test.beforeEach(async ({ page }) => {
    await loginViaApi(page, 'buero', 'buero');
  });

  test('Defekte: Mausklick auf Katalogtreffer und Enter übernimmt den vollen Wert', async ({ page }) => {
    await openFirstRepairOrder(page);

    await selectCatalogItem(page, 'Defekte', 'Glas', 'Glas gebrochen');
    await searchInput(page, 'Defekte').press('Enter');

    // Erwartung: Der zugeordnete Eintrag zeigt den vollständigen Katalogtext, nicht nur
    // den getippten Teil ("Glas").
    await expect(rows(page, 'Defekte').filter({ hasText: 'Glas gebrochen' }).first())
      .toBeVisible({ timeout: 15_000 });

    await expect
      .poll(async () => {
        const texts = (await rowTexts(page, 'Defekte')).filter((row) => row.includes('Glas'));
        return texts.length === 1 && texts[0].includes('Glas gebrochen');
      }, { timeout: 5_000 })
      .toBe(true);
  });

  test('Defekte: frei erfasster Text wird mit Enter übernommen', async ({ page }) => {
    await openFirstRepairOrder(page);

    await insertWithEnter(page, 'Defekte', 'Defekt Sonderanfertigung');
  });

  test('Defekte: Eingabe wird über den Button „Einfügen“ übernommen', async ({ page }) => {
    await openFirstRepairOrder(page);

    await insertWithButton(page, 'Defekte', 'Defekt Einfuegen-Test');
  });

  test('Arbeiten: Mausklick auf Katalogtreffer und Enter übernimmt den vollen Wert', async ({ page }) => {
    await openFirstRepairOrder(page);

    await selectCatalogItem(page, 'Arbeiten', 'Glas abgedr', 'Glas abgedrückt');
    await searchInput(page, 'Arbeiten').press('Enter');

    await expect(rows(page, 'Arbeiten').filter({ hasText: 'Glas abgedrückt' }).first())
      .toBeVisible({ timeout: 15_000 });
  });

  test('Reihenfolge: neue Einträge werden ans Ende der Liste angehängt', async ({ page }) => {
    await openFirstRepairOrder(page);

    const firstText = 'Arbeit Reihenfolge A';
    const secondText = 'Arbeit Reihenfolge B';

    await insertWithEnter(page, 'Arbeiten', firstText);
    await insertWithEnter(page, 'Arbeiten', secondText);

    // Erwartung: Der zuletzt eingefügte Eintrag steht UNTER dem zuvor eingefügten Eintrag –
    // neue Einträge werden also ans Ende gehängt und nicht an erster Stelle eingefügt.
    await expect
      .poll(async () => {
        const texts = await rowTexts(page, 'Arbeiten');
        const firstIndex = texts.findIndex((row) => row.includes(firstText));
        const secondIndex = texts.findIndex((row) => row.includes(secondText));
        return firstIndex >= 0 && secondIndex > firstIndex;
      }, { timeout: 15_000 })
      .toBe(true);
  });

  test('Reihenfolge: Einträge lassen sich per Drag&Drop umsortieren', async ({ page }) => {
    await openFirstRepairOrder(page);

    const firstText = 'Arbeit Drag A';
    const secondText = 'Arbeit Drag B';

    await insertWithEnter(page, 'Arbeiten', firstText);
    await insertWithEnter(page, 'Arbeiten', secondText);

    const firstRow = rows(page, 'Arbeiten').filter({ hasText: firstText }).first();
    const secondRow = rows(page, 'Arbeiten').filter({ hasText: secondText }).first();
    await expect(firstRow).toBeVisible();
    await expect(secondRow).toBeVisible();

    // Die zweite Zeile vor die erste Zeile ziehen.
    await secondRow.dragTo(firstRow);

    await expect
      .poll(async () => {
        const texts = await rowTexts(page, 'Arbeiten');
        const firstIndex = texts.findIndex((row) => row.includes(firstText));
        const secondIndex = texts.findIndex((row) => row.includes(secondText));
        return secondIndex >= 0 && secondIndex < firstIndex;
      }, { timeout: 15_000 })
      .toBe(true);
  });

  test('Reihenfolge: Entfernen eines Eintrags nummeriert die Liste neu', async ({ page }) => {
    await openFirstRepairOrder(page);

    const firstText = 'Arbeit Entfernen A';
    const secondText = 'Arbeit Entfernen B';

    await insertWithEnter(page, 'Arbeiten', firstText);
    await insertWithEnter(page, 'Arbeiten', secondText);

    await rows(page, 'Arbeiten').filter({ hasText: firstText }).first().locator('button').last().click();

    await expect(rows(page, 'Arbeiten').filter({ hasText: firstText })).toHaveCount(0, { timeout: 15_000 });
    await expect(rows(page, 'Arbeiten').filter({ hasText: secondText }).first()).toBeVisible();

    // Die verbleibenden Einträge werden fortlaufend neu nummeriert.
    await expect
      .poll(async () => {
        const texts = await rowTexts(page, 'Arbeiten');
        const index = texts.findIndex((row) => row.includes(secondText));
        return index >= 0 && texts[index].includes(`${index + 1}.`);
      }, { timeout: 15_000 })
      .toBe(true);
  });
});
