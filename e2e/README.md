# E2E-Tests (Playwright)

Automatisierte Browser-Tests für CamCare (Blazor Interactive Server).
Enthalten sind Regressionstests für die `RadzenAutoComplete`-Komponenten
in den Reparaturauftrags-Formularen (*Defekte*, *Mitgelieferte Komponenten*,
*Reparaturpositionen*).

## Voraussetzungen

- Die CamCare-App muss lokal unter `http://localhost:5186` laufen
  (Standardprofil `http`, Development-Umgebung).
- Die Datenbank muss Testdaten enthalten (mindestens ein Reparaturauftrag
  sowie Katalogeinträge in `Defectives` / `IncludedComponents` /
  `RepairPositions`).
- Node.js ≥ 18 und einmalig die Playwright-Browser:
  ```powershell
  cd e2e
  npm install
  npx playwright install chromium
  ```

## Tests ausführen

```powershell
cd e2e
npm test          # headless
npm run test:headed
```

Konfiguration: `e2e/playwright.config.ts`

## Was wird getestet

### `tests/mail-signature.spec.ts`

1. **Signatur-Tab** – unter *Einstellungen → Signatur* sind der Aktivierungs-Schalter,
   der HTML-Editor mit dem Platzhalter `{{Logo}}`, die Werkzeuge zum Einfügen des
   Platzhalters sowie die Logo-Dateiauswahl vorhanden.
2. **Logo-Upload** – ein Logo kann hochgeladen, gespeichert und nach dem Neuladen
   wieder angezeigt werden; anschließend lässt es sich wieder entfernen.

Der Test läuft mit der Benutzergruppe `admin` und setzt die Signatur am Ende wieder
in den Ausgangszustand (kein Logo) zurück.

### `tests/autocomplete.spec.ts`

1. **Defekte** – Mausklick auf einen Katalogtreffer übernimmt den vollen Wert
   (Regression: Es bleibt kein unvollständiger getippter Teilwert in der Zelle).
2. **Mitgelieferte Komponenten** – Mausklick auf einen Katalogtreffer übernimmt den Wert.
3. **Reparaturpositionen** – Mausklick übernimmt Beschreibung *und* Artikelnummer.
4. **Defekte (freie Eingabe)** – Text ohne Katalogtreffer wird über den
   Save-Button übernommen.

Die Tests sind bewusst **seriell** (`workers: 1`), da alle Tests gegen eine
einzelne In-Memory-App-Instanz laufen und sich parallele Edits auf denselben
Reparaturauftrag gegenseitig überschreiben würden.
