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
  - Erfassung von Defekten, mitgelieferten Komponenten und Reparaturpositionen
  - Statusverwaltung mit vollständigem Statusverlauf (History)
  - Archivierung von abgeschlossenen Aufträgen

- **Stammdatenverwaltung**
  - Kunden
  - Mitarbeiter (Techniker)
  - Logistikdienstleister
  - Reparaturpositionen (Artikelnummern, Beschreibungen)
  - Defekte
  - Mitgelieferte Komponenten
  - Reparaturauftrags-Status

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
| `admin` | `admin` | Volle Sicht und alle Rechte (inkl. Auftragserstellung); kann Passwörter aller Gruppen unter **Stammdaten → Benutzergruppen** ändern |
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
| `ASPNETCORE_ENVIRONMENT`             | ASP.NET-Umgebung (Development/Production)                            |

## API-Endpunkte

| Endpunkt                                      | Beschreibung                                                           |
| --------------------------------------------- | ---------------------------------------------------------------------- |
| `GET /health`                                 | Health-Check                                                           |
| `GET /api/datastores/{id}/file?download=true` | Datei aus dem Object Storage abrufen (mit Download- und Range-Support) |

## Mobiler Datei-Upload per QR-Code

In der Auftragsansicht (**Auftrag bearbeiten**) kann über **„Mobiler Upload"** ein zeitlich
begrenzter Upload-Link erzeugt werden (QR-Code + kopierbarer Link). Damit können z.&#8201;B.
Mitarbeiter oder Kunden mit dem Smartphone Fotos, PDF- und Videodateien direkt an den Auftrag
anhängen – ganz ohne Login in der CamCare-Oberfläche.

- Der Link zeigt auf die anonyme Seite `GET /u/{Token}`.
- Das Token ist ein HMAC-SHA256-signierter, zeitlich begrenzter Link (Standard: 120 Minuten),
  signiert mit `UploadLink__Secret`. Ohne gültiges/abgelaufenes Token wird die Seite verweigert.
- Hochgeladene Dateien landen über den vorhandenen `DataStoreService` im Object Storage (MinIO)
  und erscheinen sofort in der Dateiablage des Auftrags.

### Freigabe über Nginx Proxy Manager

Die mobile Upload-Seite wird **gezielt** über einen eigenen Reverse-Proxy-Host freigegeben
(z.&#8201;B. `upload.example.com`), nicht die gesamte CamCare-Oberfläche:

1. **Proxy Host anlegen:** Domain `upload.example.com` → Ziel `http://camcare:8080`
   (bzw. die IP des Docker-Hosts und den veröffentlichten Port).
2. **Websockets aktivieren:** Da die Upload-Seite eine **Blazor-Server**-Komponente ist,
   muss im Nginx Proxy Manager die Option **„Websockets Support"** aktiviert sein.
3. **Pfad-Allowlist** (Advanced-Config des Locations), damit über diese Domain nur die
   Upload-Seite und deren Ressourcen erreichbar sind:

   ```nginx
   # Nur Upload-Seite und benötigte Assets freigeben – alles andere blockieren.
   location ~ ^/(u/|_framework/|_blazor|js/|css/|_content/|CamCare\.png) {
       proxy_pass http://camcare:8080;
       proxy_set_header Host $host;
       proxy_set_header X-Real-IP $remote_addr;
       proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
       proxy_set_header X-Forwarded-Proto $scheme;
       proxy_http_version 1.1;
       proxy_set_header Upgrade $http_upgrade;
       proxy_set_header Connection "upgrade";
   }
   location / {
       return 403;
   }
   ```

4. **Upload-Größe begrenzen:** Passt das Limit an die maximale Dateigröße an
   (Standard 100&#8201;MB, siehe `ObjectStorage__MaxFileSizeBytes`):

   ```nginx
   client_max_body_size 110m;
   ```

5. **`UploadLink__PublicBaseUrl`** muss auf die öffentliche Domain zeigen (z.&#8201;B.
   `https://upload.example.com`), damit der QR-Code nicht auf die interne Adresse verweist.
6. CamCare erwartet, dass der Proxy die Header `X-Forwarded-For` und `X-Forwarded-Proto`
   setzt (im NPM standardmäßig der Fall). Die App vertraut diesen Headern aus privaten
   IP-Netzwerken (`10/8`, `172.16/12`, `192.168/16`, Loopback) über `UseForwardedHeaders`
   mit `ForwardedHeadersOptions`.

## Lizenz

Siehe [LICENSE.txt](LICENSE.txt).

### Eingebundene Drittbibliotheken

- **`wwwroot/js/qrcode.min.js`** – QR-Code-Generator ([`davidshimjs/qrcodejs`](https://github.com/davidshimjs/qrcodejs), MIT-Lizenz).
