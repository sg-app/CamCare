import { Page } from '@playwright/test';

/**
 * Loggt den Benutzer über den API-Endpunkt ein und überträgt den Auth-Cookie
 * in den Browser-Kontext der Seite.
 */
export async function loginViaApi(page: Page, group = 'buero', password = 'buero'): Promise<void> {
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
