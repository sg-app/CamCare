import { Page } from '@playwright/test';

/**
 * Benutzergruppe und Passwort für die API-Anmeldung. Voreingestellt sind die Standard-Zugangsdaten
 * der Gruppe `buero`; über die Umgebungsvariablen `CAMCARE_E2E_GROUP` und `CAMCARE_E2E_PASSWORD`
 * überschreibbar (z. B. wenn das Passwort in der lokalen Datenbank geändert wurde).
 */
export const DEFAULT_GROUP = process.env.CAMCARE_E2E_GROUP ?? 'buero';
export const DEFAULT_PASSWORD = process.env.CAMCARE_E2E_PASSWORD ?? 'buero';

/**
 * Loggt den Benutzer über den API-Endpunkt ein und überträgt den Auth-Cookie
 * in den Browser-Kontext der Seite.
 */
export async function loginViaApi(page: Page, group = DEFAULT_GROUP, password = DEFAULT_PASSWORD): Promise<void> {
  const response = await page.request.post('/api/auth/login', {
    data: { group, password },
  });

  if (!response.ok()) {
    throw new Error(`Login fehlgeschlagen für '${group}' (HTTP ${response.status()})`);
  }

  const setCookie = response.headers()['set-cookie'];
  if (!setCookie) {
    throw new Error('Kein Set-Cookie im Login-Response enthalten.');
  }

  const [nameValue] = setCookie.split(';');
  const [name, value] = nameValue.split('=');
  if (!name || !value) {
    throw new Error(`Cookie konnte nicht geparst werden: ${setCookie}`);
  }

  await page.context().addCookies([
    {
      name,
      value,
      domain: 'localhost',
      path: '/',
    },
  ]);
}
