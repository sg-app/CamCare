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

Die Anmeldung erfolgt über `POST /api/auth/login` mit der Gruppe `buero` und deren
Standard-Passwort. Wurde das Passwort in der lokalen Datenbank geändert, lassen sich die
Zugangsdaten überschreiben:

```powershell
$env:CAMCARE_E2E_GROUP = 'werkstatt'
$env:CAMCARE_E2E_PASSWORD = 'werk'
npm test
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

**Zuordnungen über das Grid (Mitgelieferte Komponenten, Reparaturpositionen)**

1. **Mitgelieferte Komponenten** – Mausklick auf einen Katalogtreffer übernimmt den vollen Wert.
2. **Reparaturpositionen** – Mausklick übernimmt Beschreibung *und* Artikelnummer.

**Zuordnungen über AutoComplete (Defekte, Ausgeführte Arbeiten)**

Die drei Wege, auf denen ein Eintrag zugeordnet wird, werden jeweils einzeln geprüft:

3. **Pfeiltasten + Enter** – der im Dropdown markierte (`aria-selected="true"`) Katalogtreffer
   wird mit Enter übernommen (voller Katalogtext, genau ein Eintrag, Feld danach leer).
4. **Maus** – ein Klick auf den Katalogtreffer übernimmt ihn sofort, ohne Enter/„Einfügen“.
5. **Eingabe ohne Auswahl** – Freitext wird mit Enter bzw. über den Button „Einfügen“ angelegt;
   entspricht die Eingabe exakt einem Katalogeintrag, wird dieser übernommen; ein bereits
   zugeordneter Eintrag wird nicht doppelt angelegt.
6. **Reihenfolge** – neue Einträge werden ans Ende gehängt, per Drag&Drop umsortiert und beim
   Entfernen eines Eintrags bleibt die Reihenfolge der übrigen erhalten.

Die Zuordnung wird jeweils über den exakten Beschreibungstext (`getByText(..., { exact: true })`)
geprüft, damit auch geprüft ist, dass kein doppelter bzw. nur teilweise übernommener Eintrag
entsteht.

Die Tests sind bewusst **seriell** (`workers: 1`), da alle Tests gegen eine
einzelne In-Memory-App-Instanz laufen und sich parallele Edits auf denselben
Reparaturauftrag gegenseitig überschreiben würden.
