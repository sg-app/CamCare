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
 * Öffnet das Dropdown einer Zuordnung, tippt den Suchtext und wartet auf den Katalogtreffer.
 * Liefert das Eingabefeld und den Treffer zurück.
 */
async function searchCatalogItem(page: Page, name: string, searchText: string, itemText: string) {
  const input = searchInput(page, name);
  await input.click();
  await input.pressSequentially(searchText, { delay: 100 });

  const item = page.locator('.rz-autocomplete-list-item', { hasText: itemText }).first();
  await expect(item).toBeVisible({ timeout: 15_000 });

  return { input, item };
}

/**
 * Tippt einen Suchtext, markiert den ersten Katalogtreffer mit den Pfeiltasten und liefert
 * das Eingabefeld sowie den vollständigen Text des markierten Treffers zurück.
 */
async function highlightFirstCatalogItem(page: Page, name: string, searchText: string) {
  const input = searchInput(page, name);
  await input.click();
  await input.pressSequentially(searchText, { delay: 100 });

  await expect(page.locator('.rz-autocomplete-list-item').first()).toBeVisible({ timeout: 15_000 });

  // Mit den Pfeiltasten wird der Eintrag markiert (aria-selected wird vom Server gesetzt).
  await input.press('ArrowDown');
  const highlighted = page.locator('.rz-autocomplete-list-item[aria-selected="true"]').first();
  await expect(highlighted).toBeVisible({ timeout: 15_000 });

  return { input, expected: await visibleText(highlighted) };
}

/** Sichtbarer Text eines Elements, auf einfache Leerzeichen normalisiert. */
async function visibleText(locator: Locator): Promise<string> {
  return (await locator.innerText()).replace(/\s+/g, ' ').trim();
}

/**
 * Zugeordnete Einträge einer Zuordnung mit exakt diesem Beschreibungstext. Bewusst auf die
 * Zeilen eingegrenzt: Die (ausgeblendeten) Einträge des AutoComplete-Dropdowns enthalten
 * denselben Text.
 */
function entries(page: Page, name: string, text: string): Locator {
  return assignment(page, name).locator('.camcare-assignment-row').getByText(text, { exact: true });
}

/**
 * Prüft, dass der Eintrag dem Auftrag genau einmal zugeordnet ist (also nicht doppelt
 * angelegt wurde). Die Prüfung erfolgt auf den exakten Text des Eintrags.
 */
async function expectEntryOnce(page: Page, name: string, text: string): Promise<void> {
  await expect(entries(page, name, text).first()).toBeVisible({ timeout: 15_000 });
  await expect(entries(page, name, text)).toHaveCount(1, { timeout: 15_000 });
}

/** Prüft, dass dem Auftrag kein Eintrag mit genau diesem Text zugeordnet ist. */
async function expectNoEntry(page: Page, name: string, text: string): Promise<void> {
  await expect(entries(page, name, text)).toHaveCount(0, { timeout: 15_000 });
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
    await loginViaApi(page);
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
    await loginViaApi(page);
  });

  test('Defekte: Auswahl mit den Pfeiltasten und Enter fügt den Katalogtreffer ein', async ({ page }) => {
    await openFirstRepairOrder(page);

    const { input, expected } = await highlightFirstCatalogItem(page, 'Defekte', 'Glas');

    await input.press('Enter');

    // Übernommen wird der vollständige Katalogtext – nicht der nur getippte Teil ("Glas").
    await expectEntryOnce(page, 'Defekte', expected);
    await expectNoEntry(page, 'Defekte', 'Glas');

    // Nach dem Übernehmen ist das Eingabefeld wieder leer.
    await expect(input).toHaveValue('');
  });

  test('Defekte: Auswahl mit der Maus fügt den Katalogtreffer ohne Enter ein', async ({ page }) => {
    await openFirstRepairOrder(page);

    const { input, item } = await searchCatalogItem(page, 'Defekte', 'Glas', 'Glas gebrochen');
    const expected = await visibleText(item);

    await item.click();

    // Der Mausklick übernimmt den Eintrag direkt – ohne Enter und ohne „Einfügen“.
    await expectEntryOnce(page, 'Defekte', expected);
    await expect(input).toHaveValue('');
  });

  test('Defekte: Eingabe ohne Auswahl legt einen neuen Eintrag an', async ({ page }) => {
    await openFirstRepairOrder(page);

    const text = 'Defekt Sonderanfertigung';
    await insertWithEnter(page, 'Defekte', text);

    // Der Freitext wird genau einmal angelegt (kein Duplikat durch das nachlaufende
    // change-Event des AutoComplete).
    await expectEntryOnce(page, 'Defekte', text);
    await expect(searchInput(page, 'Defekte')).toHaveValue('');
  });

  test('Defekte: Eingabe ohne Auswahl übernimmt einen exakt passenden Katalogtreffer', async ({ page }) => {
    await openFirstRepairOrder(page);

    // Der Text entspricht genau einem Katalogeintrag, wird aber nicht im Dropdown ausgewählt.
    const input = searchInput(page, 'Defekte');
    await input.click();
    await input.pressSequentially('Glas gebrochen', { delay: 100 });
    await input.press('Enter');

    await expectEntryOnce(page, 'Defekte', 'Glas gebrochen');
  });

  test('Defekte: bereits zugeordneter Eintrag wird nicht doppelt angelegt', async ({ page }) => {
    await openFirstRepairOrder(page);

    const text = 'Defekt Doppelt';
    await insertWithEnter(page, 'Defekte', text);
    await insertWithEnter(page, 'Defekte', text);

    await expectEntryOnce(page, 'Defekte', text);
  });

  test('Defekte: Eingabe wird über den Button „Einfügen“ übernommen', async ({ page }) => {
    await openFirstRepairOrder(page);

    await insertWithButton(page, 'Defekte', 'Defekt Einfuegen-Test');
  });

  test('Arbeiten: Auswahl mit den Pfeiltasten und Enter fügt den Katalogtreffer ein', async ({ page }) => {
    await openFirstRepairOrder(page);

    const { input, expected } = await highlightFirstCatalogItem(page, 'Arbeiten', 'Glas abgedr');

    await input.press('Enter');

    await expectEntryOnce(page, 'Arbeiten', expected);
    await expect(input).toHaveValue('');
  });

  test('Arbeiten: Auswahl mit der Maus fügt den Katalogtreffer ohne Enter ein', async ({ page }) => {
    await openFirstRepairOrder(page);

    const { item } = await searchCatalogItem(page, 'Arbeiten', 'Glas abgedr', 'Glas abgedrückt');

    await item.click();

    await expectEntryOnce(page, 'Arbeiten', 'Glas abgedrückt');
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

  test('Reihenfolge: Entfernen eines Eintrags behält die Reihenfolge der übrigen Einträge', async ({ page }) => {
    await openFirstRepairOrder(page);

    const firstText = 'Arbeit Entfernen A';
    const middleText = 'Arbeit Entfernen B';
    const lastText = 'Arbeit Entfernen C';

    await insertWithEnter(page, 'Arbeiten', firstText);
    await insertWithEnter(page, 'Arbeiten', middleText);
    await insertWithEnter(page, 'Arbeiten', lastText);

    await rows(page, 'Arbeiten').filter({ hasText: middleText }).first().locator('button').last().click();

    await expect(rows(page, 'Arbeiten').filter({ hasText: middleText }))
      .toHaveCount(0, { timeout: 15_000 });

    // Die übrigen Einträge behalten ihre Reihenfolge (A steht weiterhin vor C).
    await expect
      .poll(async () => {
        const texts = await rowTexts(page, 'Arbeiten');
        const firstIndex = texts.findIndex((row) => row.includes(firstText));
        const lastIndex = texts.findIndex((row) => row.includes(lastText));
        return firstIndex >= 0 && lastIndex > firstIndex;
      }, { timeout: 15_000 })
      .toBe(true);
  });
});
