# CamCare

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

## Docker-Deployment

Die `docker-compose.yml` startet die vollständige Umgebung:

- **camcare-sql**: SQL Server 2022 Express
- **minio**: MinIO Object Storage (Ports 9000/9001)
- **camcare**: Die CamCare-Anwendung (Port 8085)

```bash
# SA_PASSWORD als Umgebungsvariable setzen
export SA_PASSWORD="DeinSicheresPasswort"

# Alle Dienste starten
docker compose up -d
```

Die Anwendung ist danach unter `http://localhost:8085` erreichbar.

### Wichtige Umgebungsvariablen

| Variable                             | Beschreibung                                    |
| ------------------------------------ | ----------------------------------------------- |
| `ConnectionStrings__Default`         | SQL-Server-Verbindung für die CamCare-Datenbank |
| `ConnectionStrings__AmicronDatabase` | Firebird-Datenbank (Amicron)                    |
| `ConnectionStrings__DefaultKrd`      | SQL-Server-Verbindung für die KRD-Altdaten      |
| `ObjectStorage__Endpoint`            | Endpunkt des MinIO/Object-Storage-Servers       |
| `ASPNETCORE_ENVIRONMENT`             | ASP.NET-Umgebung (Development/Production)       |

## API-Endpunkte

| Endpunkt                                      | Beschreibung                                                           |
| --------------------------------------------- | ---------------------------------------------------------------------- |
| `GET /health`                                 | Health-Check                                                           |
| `GET /api/datastores/{id}/file?download=true` | Datei aus dem Object Storage abrufen (mit Download- und Range-Support) |

## Lizenz

Siehe [LICENSE.txt](LICENSE.txt).
