<p align="center">
  <img src="CamCare/wwwroot/CamCare.png" alt="CamCare Logo" width="200"/>
</p>

<h1 align="center">CamCare</h1>

<p align="center">
  <a href="https://github.com/sg-app/CamCare/releases">
    <img alt="GitHub Release" src="https://img.shields.io/github/v/release/sg-app/CamCare?include_prereleases&label=Release&style=for-the-badge"/>
  </a>
  <a href="https://github.com/sg-app/CamCare/blob/main/LICENSE.txt">
    <img alt="License" src="https://img.shields.io/github/license/sg-app/CamCare?style=for-the-badge"/>
  </a>
  <a href="https://github.com/sg-app/CamCare/commits/main">
    <img alt="Last Commit" src="https://img.shields.io/github/last-commit/sg-app/CamCare?style=for-the-badge"/>
  </a>
  <img alt=".NET 10" src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge"/>
</p>

**CamCare** ist eine webbasierte Anwendung zur Verwaltung von Kamera-Reparaturaufträgen. Sie unterstützt den gesamten Prozess von der Erfassung eingehender Geräte über die Reparaturdurchführung bis hin zur Auslieferung – inklusive Stammdatenverwaltung, Statusverfolgung, Logistik und Dokumentenablage.

## Features

- **Reparaturauftragsverwaltung**
  - Erfassung und Bearbeitung von Reparaturaufträgen
  - Zuordnung von Kunden, Seriennummern und Geräten
  - Nachträgliche Änderung des Auftragskunden durch `admin` (Lieferanschrift und E-Mail-Adressen werden dabei zurückgesetzt)
  - **Defekte** und **ausgeführte Arbeiten** werden über ein AutoComplete erfasst (Suche im
    Katalog oder freie Erfassung). Ein Katalogtreffer wird direkt bei der Auswahl übernommen –
    per **Mausklick** oder per **Pfeiltasten + Enter**. Eine Eingabe ohne Auswahl wird mit
    **Enter** oder dem Button **Einfügen** angelegt bzw. als Katalogeintrag übernommen, wenn sie
    exakt dessen Beschreibung entspricht; bereits zugeordnete Einträge werden nicht doppelt angelegt
  - Gespeicherte Reihenfolge dieser Einträge (`SortOrder`); nachträgliche Änderung per Drag&Drop
  - Mitgelieferte Komponenten und Reparaturpositionen werden weiterhin über eine Tabelle erfasst
  - Statusverwaltung mit vollständigem Statusverlauf (History)
  - Archivierung von abgeschlossenen Aufträgen

- **Stammdatenverwaltung**
  - Kunden
  - Mitarbeiter (Techniker)
  - Logistikdienstleister
  - Reparaturpositionen (Artikelnummern, Beschreibungen)
  - Defekte
  - Mitgelieferte Komponenten
  - Reparaturauftrags-Status (die Flags „Standard", „Abschluss" und „Eingetroffen" können jeweils nur einem Status zugeordnet werden)

- **Logistik & Verpackung**
  - Versandart (Eigenversand durch Kunden oder Versandunternehmen)
  - Verpackungstyp mit Maßen (Länge, Breite, Höhe) und Kommentar
  - Zuordnung von Logistikdienstleistern
  - Auftrags-, Angebots- und Lieferscheinnummern

- **Integrationen**
  - **Amicron**: Anbindung an das Amicron-System (Firebird-Datenbank) für Reparaturpositionen
  - **KRD-Altdaten**: Lesender Zugriff auf historische Auftragsdaten aus der KRD-Datenbank

- **Dateiablage**
  - Anhängen von Dateien an Reparaturaufträge (z. B. Fotos, Dokumente)
  - Speicherung in einem S3-kompatiblen Object Storage (MinIO)
  - Bereitstellung über einen API-Endpunkt mit Download- und Range-Support
  - Mobiler Upload per QR-Code/Link direkt aus der Auftragsansicht (ohne Login)

- **Weitere Funktionen**
  - Responsive Blazor-UI mit Radzen-Komponenten
  - State Management mit Fluxor
  - Validierung mit FluentValidation
  - Health-Check-Endpunkt (`/health`)
  - Strukturierte Protokollierung mit NLog

## Technologie-Stack

| Bereich             | Technologie                                  |
| ------------------- | -------------------------------------------- |
| Framework           | .NET 10, ASP.NET Core (Blazor Server)        |
| UI                  | Radzen.Blazor                                |
| State Management    | Fluxor (inkl. Redux DevTools im Debug-Modus) |
| Datenbank           | SQL Server (Entity Framework Core)           |
| Altsystem-Anbindung | Amicron (Firebird), KRD (MSSQL)              |
| Object Storage      | MinIO (S3-kompatibel, AWSSDK.S3)             |
| Validierung         | FluentValidation                             |
| Logging             | NLog                                         |
| Container           | Docker, Docker Compose                       |

## Projektstruktur

```
CamCare/
├── Components/          # Blazor-Komponenten (Pages, Layout, RepairOrderComponents)
├── Domain/              # Entitäten (RepairOrder, RepairPosition, Defective, ...)
├── Extensions/          # Erweiterungsmethoden (Query, String)
├── Interfaces/          # Service- und Persistenz-Schnittstellen
├── Migrations/          # EF-Core-Migrationen
├── Models/              # ViewModels und Validatoren
├── Options/             # Konfigurationsoptionen (z. B. ObjectStorage)
├── Persistence/         # DbContexts (AppDbContext, KrdDbContext) und Registrierung
├── Services/            # Services (RepairOrder, Masterdata, Amicron, MinIO, ...)
├── Store/               # Fluxor-State (RepairOrderFeature)
└── wwwroot/             # Statische Dateien (CSS, JS)
```

## Erste Schritte (Lokale Entwicklung)

### Voraussetzungen

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (für SQL Server und MinIO)
- Optional: [Visual Studio 2022+](https://visualstudio.microsoft.com/) oder VS Code

### Konfiguration

Die Verbindungszeichenfolgen und Optionen werden über `appsettings.json` bzw. `appsettings.Development.json` konfiguriert:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=CamCare;User Id=sa;Password=...;TrustServerCertificate=True",
    "AmicronDatabase": "PathToFireBirdDb",
    "DefaultKrd": "Server=localhost;Database=Krd;User Id=sa;Password=...;TrustServerCertificate=True"
  },
  "ObjectStorage": {
    "Endpoint": "http://localhost:9000",
    "AccessKey": "...",
    "SecretKey": "...",
    "BucketName": "..."
  },
  "Mail": {
    "IsEnabled": false,
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "Security": "StartTls",
    "UserName": "",
    "Password": "",
    "TimeoutSeconds": 30,
    "FromAddress": "noreply@example.com",
    "FromName": "CamCare Reparatur"
  }
}
```

### Datenbank starten

```bash
docker compose up -d camcare-sql minio
```

### Anwendung starten

```bash
dotnet run --project CamCare
```

Die Anwendung ist anschließend unter `https://localhost:5001` (bzw. dem in `launchSettings.json` konfigurierten Port) erreichbar.

Beim Start werden offene EF-Core-Migrationen automatisch angewendet und Stammdaten initialisiert.

## Anmeldung & Benutzergruppen

Die Anwendung nutzt eine einfache Gruppenanmeldung (ohne einzelne Benutzerkonten). Beim ersten Start werden die drei Benutzergruppen mit folgenden Standard-Passwörtern angelegt:

| Benutzergruppe | Standard-Passwort | Rechte |
|---|---|---|
| `admin` | `admin` | Volle Sicht und alle Rechte (inkl. Auftragserstellung, nachträglicher Änderung des Auftragskunden und Löschen von Aufträgen); kann Passwörter aller Gruppen unter **Stammdaten → Benutzergruppen** ändern |
| `buero` | `buero` | Kann neue Aufträge anlegen („Auftrag erstellen") |
| `werkstatt` | `werk` | Zugriff auf Aufträge und Stammdaten, keine Auftragserstellung |

Die Anmeldung erfolgt über einen Anmeldedialog (Benutzergruppe + Passwort). Eine Benutzergruppe bleibt angemeldet, bis über „Abmelden" abgemeldet wird. Nach dem ersten Anmelden sollten die Standard-Passwörter über **Stammdaten → Benutzergruppen** (nur `admin`) geändert werden. Die Passwörter werden gesalzen und gehasht (PBKDF2) gespeichert.

## Docker-Deployment

Die `docker-compose.yml` startet die vollständige Umgebung:

- **camcare-sql**: SQL Server 2022 Express
- **minio**: MinIO Object Storage (Ports 9000/9001)
- **camcare**: Die CamCare-Anwendung (Port 8085)

```bash
# SA_PASSWORD als Umgebungsvariable setzen
export SA_PASSWORD="DeinSicheresPasswort"

# Geheimer Schlüssel für die Upload-Link-Signierung (mindestens 16 Zeichen)
export UPLOAD_LINK_SECRET="EinSehrLangesGeheimesPasswort"

# Öffentliche Basis-URL, unter der CamCare (bzw. der mobile Upload) erreichbar ist
export UPLOAD_LINK_PUBLIC_BASE_URL="https://upload.example.com"

# Alle Dienste starten
docker compose up -d
```

Die Anwendung ist danach unter `http://localhost:8085` erreichbar.

### Wichtige Umgebungsvariablen

| Variable                             | Beschreibung                                                       |
| ------------------------------------ | ------------------------------------------------------------------ |
| `ConnectionStrings__Default`         | SQL-Server-Verbindung für die CamCare-Datenbank                     |
| `ConnectionStrings__AmicronDatabase` | Firebird-Datenbank (Amicron)                                        |
| `ConnectionStrings__DefaultKrd`      | SQL-Server-Verbindung für die KRD-Altdaten                           |
| `ObjectStorage__Endpoint`            | Endpunkt des MinIO/Object-Storage-Servers                            |
| `UploadLink__Secret`                 | Geheimer Schlüssel zur Signierung der mobilen Upload-Links           |
| `UploadLink__TokenLifetimeMinutes`   | Gültigkeitsdauer eines Upload-Links in Minuten (Standard: 120)       |
| `UploadLink__PublicBaseUrl`          | Öffentliche Basis-URL für QR-Code/Link des mobilen Uploads           |
| `Mail__IsEnabled`                    | Hauptschalter für den automatischen E-Mail-Versand (Standard: `false`) |
| `Mail__SmtpHost` / `Mail__SmtpPort`  | SMTP-Server und Port (Standard-Port: `587`)                          |
| `Mail__Security`                     | Verschlüsselung: `None`, `StartTls` oder `SslOnConnect` (Standard: `StartTls`) |
| `Mail__FromAddress` / `Mail__FromName` | Absenderadresse und -name der versendeten E-Mails                  |
| `Mail__UserName` / `Mail__Password`  | Zugangsdaten für die SMTP-Authentifizierung (optional)              |
| `ASPNETCORE_ENVIRONMENT`             | ASP.NET-Umgebung (Development/Production)                            |

## API-Endpunkte

| Endpunkt                                      | Beschreibung                                                           |
| --------------------------------------------- | ---------------------------------------------------------------------- |
| `GET /health`                                 | Health-Check                                                           |
| `GET /api/datastores/{id}/file?download=true` | Datei aus dem Object Storage abrufen (mit Download- und Range-Support) |
| `GET /u/{token}`                              | Anonyme Mobile-Upload-Seite (statisches HTML, kein WebSocket)          |
| `POST /api/upload/{token}`                    | Multipart-Upload für einen Auftrag (bild/PDF/Video, max. 100 MB)       |

## Mobiler Datei-Upload per QR-Code

In der Auftragsansicht (**Auftrag bearbeiten**) kann über **„Mobiler Upload"** ein zeitlich
begrenzter Upload-Link erzeugt werden (QR-Code + kopierbarer Link). Damit können z.&#8201;B.
Mitarbeiter oder Kunden mit dem Smartphone Fotos, PDF- und Videodateien direkt an den Auftrag
anhängen – ganz ohne Login in der CamCare-Oberfläche.

- Der Link zeigt auf die anonyme Seite `GET /u/{Token}`.
- Das Token ist ein HMAC-SHA256-signierter, zeitlich begrenzter Link (Standard:
  `UploadLink__TokenLifetimeMinutes`, siehe `appsettings.json`), signiert mit `UploadLink__Secret`.
  **Ohne gültiges/abgelaufenes Token liefert die Seite sowie die Upload-API `404 Not Found`.**
- Die Upload-Seite ist **bewusst nicht-interaktiv** (reines HTML + Vanilla-JS, kein
  Blazor-Circuit/WebSocket): Die Dateien werden per `fetch()` als `multipart/form-data` an
  `POST /api/upload/{token}` übertragen – dadurch funktioniert der Upload zuverlässig auch hinter
  Reverse-Proxies.
- Hochgeladene Dateien landen über den vorhandenen `DataStoreService` im Object Storage (MinIO)
  und erscheinen sofort in der Dateiablage des Auftrags.
- Geöffnete Editor-Ansichten aktualisieren ihre Dateiliste **live**: Der Upload-Endpunkt
  publiziert ein `RepairOrderChangedEvent` (Typ `FileUploaded`) über den In-Process-Change-Bus
  (`RepairOrderChangeBroadcaster`). `<DataAssign />` abonniert dieses Event und lädt die Dateien
  des Auftrags neu.

### Freigabe über Nginx Proxy Manager

Die mobile Upload-Seite wird **gezielt** über einen eigenen Reverse-Proxy-Host freigegeben
(z.&#8201;B. `upload.example.com`), nicht die gesamte CamCare-Oberfläche:

1. **Proxy Host anlegen:** Domain `upload.example.com` → Ziel `http://camcare:8080`
   (bzw. die IP des Docker-Hosts und den veröffentlichten Port).
2. **Websockets sind NICHT erforderlich:** Die Upload-Seite ist eine statische HTML-Seite und
   die Upload-API ein klassischer HTTP-POST-Endpunkt. Die Option **„Websockets Support"** im
   Nginx Proxy Manager kann aus bleiben.
3. **Pfad-Allowlist** (Advanced-Config des Locations), damit über diese Domain nur die
   Upload-Seite und die Upload-API erreichbar sind – alles andere blockieren:

   ```nginx
   # Nur Upload-Seite und Upload-API freigeben – alles andere blockieren.
   location ~ ^/(u/|api/upload/) {
       proxy_pass http://camcare:8080;
       proxy_set_header Host $host;
       proxy_set_header X-Real-IP $remote_addr;
       proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
       proxy_set_header X-Forwarded-Proto $scheme;
       proxy_http_version 1.1;
   }
   location / {
       return 403;
   }
   ```

4. **Upload-Größe begrenzen:** Passt das Limit an die maximale Dateigröße an
   (Standard 100&#8201;MB, siehe `ObjectStorage__MaxFileSizeBytes`; Kestrel beachtet
   zusätzlich `client_max_body_size` des Proxys):

   ```nginx
   client_max_body_size 110m;
   ```

5. **`UploadLink__PublicBaseUrl`** muss auf die öffentliche Domain zeigen (z.&#8201;B.
   `https://upload.example.com`), damit der QR-Code nicht auf die interne Adresse verweist.
6. CamCare erwartet, dass der Proxy die Header `X-Forwarded-For` und `X-Forwarded-Proto`
   setzt (im NPM standardmäßig der Fall). Die App vertraut diesen Headern aus privaten
   IP-Netzwerken (`10/8`, `172.16/12`, `192.168/16`, Loopback) über `UseForwardedHeaders`
   mit `ForwardedHeadersOptions`.
7. **Abgelaufene/ungültige Tokens** beantworten sowohl `GET /u/{token}` als auch
   `POST /api/upload/{token}` mit **HTTP 404** – der Upload-Host verrät dadurch nichts über
   die Existenz eines Auftrags und alte QR-Codes werden als „Link abgelaufen" angezeigt.

## Automatischer E-Mail-Versand

Der E-Mail-Versand wird über den Abschnitt **`Mail`** der `appsettings.json` (bzw. über
Umgebungsvariablen wie `Mail__SmtpHost`) konfiguriert – die SMTP-Zugangsdaten werden **nicht**
in der Datenbank gespeichert. Unter **Einstellungen → EMail** (nur für die Benutzergruppe
`admin`) werden die wirksamen Werte nur lesend angezeigt; dort werden die E-Mail-Vorlagen und
das Versandprotokoll verwaltet. Unter **Einstellungen → Signatur** wird die E-Mail-Signatur
inkl. Logo gepflegt:

- **SMTP-Server**: Server, Port, Verschlüsselung (`None` / `StartTls` / `SslOnConnect`),
  Absenderadresse und -name sowie optional Benutzername/Passwort und Zeitlimit.
  Der Versand erfolgt als HTML-Mail über **MailKit**.
- **Hauptschalter**: `Mail:IsEnabled` schaltet den automatischen Statusversand ein bzw. aus
  (Standard: `false`). Der Testversand ist auch bei deaktiviertem Hauptschalter möglich.
- **Automatischer Statusversand**: Es können **beliebig viele E-Mail-Vorlagen** angelegt werden –
  jeweils für einen Status des Reparaturauftrags (z. B. eine Vorlage für **„Eingetroffen"** und eine
  für **„Versendet"**); auch mehrere Vorlagen für denselben Status sind möglich. Wechselt ein
  Reparaturauftrag in einen Status, werden alle für diesen Status **aktiven** Vorlagen an alle im
  Auftrag hinterlegten E-Mail-Adressen versendet. Betreff und Inhalt (HTML) werden mit dem
  **RadzenHtmlEditor** bearbeitet und können Platzhalter in der Schreibweise `{{Token}}` enthalten
  (z. B. `{{OrderNumber}}`, `{{SerialNumber}}`, `{{Status}}`, `{{CustomerName}}`,
  `{{Defects}}`, `{{DeliveryAddressCity}}`). Die Platzhalter werden beim Versand durch die
  Werte des Auftrags ersetzt (HTML-kodiert); unbekannte Platzhalter bleiben unverändert
  stehen. Eine Übersicht aller Platzhalter inkl. Beispielwerten ist direkt auf der Seite
  verfügbar. Neue Vorlagen werden für die Status „Eingetroffen" und „Versendet" bereits
  als deaktivierte Beispielvorlagen angelegt.

  Neben einfachen Platzhaltern stehen **Funktionen** zur Verfügung (siehe
  `MailTemplateParser`), mit denen sich z. B. leere Zeilen vermeiden lassen:

  | Schreibweise | Bedeutung |
  | --- | --- |
  | `{{Token}}` | Wert des Platzhalters (leer, wenn kein Wert vorhanden ist) |
  | `{{Token\|Ersatzwert}}` | Ersatzwert, wenn der Wert leer ist (wird wie Vorlagen-HTML ausgegeben) |
  | `{{#if Token}} … {{/if}}` | Inhalt nur bei gefülltem Wert, optional mit `{{else}}` |
  | `{{#unless Token}} … {{/unless}}` | Inhalt nur bei leerem Wert |
  | `{{! Kommentar}}` | wird nicht ausgegeben |

  Beispiel – die Lieferanschrift wird nur ausgegeben, wenn sie gepflegt ist
  (statt einer leeren Zeile):

  ```html
  {{#if DeliveryAddressCity}}<p>Lieferanschrift:<br />{{DeliveryAddressRecipient}}<br />
  {{DeliveryAddressStreet}}<br />{{DeliveryAddressCity}}</p>{{/if}}
  ```

  Die Blockstruktur muss vollständig sein: Fehlt z. B. `{{/if}}`, wird die Vorlage wie
  Text behandelt – Platzhalter funktionieren weiterhin, die Block-Platzhalter bleiben
  sichtbar stehen, damit der Fehler auffällt. Zum Einfügen von Platzhaltern und
  Bedingungsblöcken stehen in der Bearbeitungsleiste des HTML-Editors eigene
  Werkzeuge sowie Schaltflächen unterhalb des Inhalts zur Verfügung.
- **Testversand**: Die aktuell bearbeitete Vorlage kann unabhängig vom Hauptschalter an eine manuell
  eingegebene Adresse getestet werden. Optional werden dabei die Daten eines bestehenden Auftrags
  eingesetzt, ansonsten Beispieldaten. Ohne ausgewählte Vorlage wird eine einfache Test-E-Mail
  versendet (Prüfung der SMTP-Einstellungen).
- **Signatur mit Logo** (Einstellungen → Signatur): Eine Signatur wird an **jede** über CamCare
  versendete E-Mail angehängt – sowohl beim automatischen Statusversand als auch beim Testversand.
  - Der Inhalt wird als HTML gepflegt (RadzenHtmlEditor). Der Platzhalter `{{Logo}}` markiert die
    Stelle, an der das Logo ausgegeben wird; enthält die Signatur keinen Platzhalter, wird das Logo
    an das Ende der Signatur angehängt.
  - Das Logo wird als Bilddatei (z. B. PNG, maximal 2&#8201;MB) in den Einstellungen hinterlegt und in
    der Datenbank gespeichert. Beim Versand wird es **in die E-Mail eingebettet** (Content-Id
    `cid:…`) und nicht über eine externe URL geladen, damit es auch bei blockierten externen Bildern
    angezeigt wird. Die Breite wird in der E-Mail auf 220 Pixel begrenzt.
  - Über den Schalter **„Signatur an versendete E-Mails anhängen“** wird die Signatur ein- bzw.
    ausgeschaltet. Eine Standard-Signatur wird bei der Installation angelegt (zunächst deaktiviert).

**Kein Mehrfachversand:** Jeder Versand wird im Versandprotokoll (`RepairOrderMailLogs`)
festgehalten. Für einen Auftrag und eine Vorlage wird höchstens **eine erfolgreich
versendete** E-Mail zugelassen – auch wenn der Status mehrfach geändert wird (z. B.
Status A → „Eingetroffen" → Status B → „Eingetroffen"). Fehlgeschlagene Versuche werden
ebenfalls protokolliert, blockieren aber keinen erneuten Versuch bei einer späteren
Statusänderung. Ein Mailfehler beeinflusst das Speichern des Auftrags nicht (er wird
lediglich protokolliert und im Versandprotokoll angezeigt) und verhindert auch nicht den
Versand der übrigen Vorlagen.

> **Hinweis:** Das SMTP-Passwort steht im Abschnitt `Mail` der `appsettings.json` bzw. in einer
> Umgebungsvariable (`Mail__Password`). Die Konfigurationsdatei ist entsprechend einzuschränken;
> in Produktion sollte das Passwort über eine sichere Umgebungsvariable gesetzt werden.

## Lizenz

Siehe [LICENSE.txt](LICENSE.txt).

### Eingebundene Drittbibliotheken

- **`wwwroot/js/qrcode.min.js`** – QR-Code-Generator ([`davidshimjs/qrcodejs`](https://github.com/davidshimjs/qrcodejs), MIT-Lizenz).
