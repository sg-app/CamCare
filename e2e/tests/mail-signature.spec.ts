import { test, expect, Page } from '@playwright/test';
import * as path from 'path';
import { loginViaApi } from './helpers';

/** Beispiel-Logo aus dem Web-Root der Anwendung. */
const logoFilePath = path.join(__dirname, '..', '..', 'CamCare', 'wwwroot', 'CamCare.png');

/**
 * Wechselt auf der Einstellungsseite zum Tab „Signatur“.
 *
 * Der Blazor-Circuit ist beim ersten Klick ggf. noch nicht verbunden; der Klick wird
 * deshalb so lange wiederholt, bis der Inhalt des Tabs gerendert ist.
 */
async function openSignatureTab(page: Page): Promise<void> {
  await page.goto('/Settings');

  const tab = page.getByRole('tab', { name: 'Signatur' });
  const heading = page.getByRole('heading', { name: 'E-Mail-Signatur' });

  await expect
    .poll(
      async () => {
        if ((await heading.count()) === 0) {
          await tab.click().catch(() => undefined);
        }
        return heading.count();
      },
      { timeout: 30_000, intervals: [500, 1000, 1500] },
    )
    .toBeGreaterThan(0);

  await expect(heading).toBeVisible();
}

test.describe('E-Mail-Signatur (Einstellungen)', () => {
  test.beforeEach(async ({ page }) => {
    await loginViaApi(page, 'admin', 'admin');
  });

  test('Signatur-Tab bietet Editor, Logo-Upload und Aktivierungs-Schalter', async ({ page }) => {
    await openSignatureTab(page);

    // Aktivierungs-Schalter und HTML-Editor mit Logo-Platzhalter
    await expect(page.getByText('Signatur an versendete E-Mails anhängen')).toBeVisible();
    await expect(page.locator('.rz-html-editor-content').first()).toContainText('{{Logo}}');

    // Werkzeuge zum Einfügen des Logo-Platzhalters
    await expect(page.getByRole('button', { name: 'Logo der Signatur an der Cursorposition einfügen' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Logo-Platzhalter an das Ende der Signatur anfügen' })).toBeVisible();

    // Logo-Dateiauswahl
    await expect(page.locator('input[type="file"]')).toHaveCount(1);
  });

  test('Logo hochladen, speichern und wieder entfernen', async ({ page }) => {
    await openSignatureTab(page);

    // Logo hochladen
    await page.locator('input[type="file"]').setInputFiles(logoFilePath);
    await expect(page.getByText(/Hinterlegt: CamCare\.png/)).toBeVisible({ timeout: 15_000 });
    await expect(page.getByAltText('Vorschau des Logos')).toBeVisible();

    // Speichern
    await page.getByRole('button', { name: 'Signatur speichern' }).click();
    await expect(page.getByText('Die Signatur wurde gespeichert.')).toBeVisible({ timeout: 15_000 });

    // Nach dem Neuladen ist das Logo weiterhin hinterlegt
    await openSignatureTab(page);
    await expect(page.getByText('Logo hinterlegt')).toBeVisible({ timeout: 15_000 });
    await expect(page.getByText(/Hinterlegt: CamCare\.png/)).toBeVisible();
    await expect(page.getByAltText('Vorschau des Logos')).toBeVisible();

    // Aufräumen: Logo wieder entfernen und speichern
    await page.getByRole('button', { name: 'Hinterlegtes Logo entfernen' }).click();
    await page.getByRole('button', { name: 'Signatur speichern' }).click();
    await expect(page.getByText('Die Signatur wurde gespeichert.')).toBeVisible({ timeout: 15_000 });

    await openSignatureTab(page);
    await expect(page.getByText('Es ist kein Logo hinterlegt.')).toBeVisible({ timeout: 15_000 });
  });
});
