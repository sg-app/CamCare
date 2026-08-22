# Copilot Instructions for CamCare

## Project Overview

CamCare is a **Blazor Interactive Server** application (ASP.NET Core 8) for managing camera repair orders. It uses **Fluxor** (Redux pattern) for state management, **Radzen.Blazor** for UI components, **Entity Framework Core** for data access, and **FluentValidation** for input validation.

## Architecture & Patterns

### Solution Structure

- **CamCare/** - Main Blazor web application project
  - **Components/** - Blazor components (Pages, Layout, Common, RepairOrderComponents)
  - **Domain/** - Entity classes (EF Core models)
  - **Models/** - ViewModels and DTOs
  - **Services/** - Business logic services
  - **Interfaces/** - Service and persistence interfaces
  - **Persistence/** - EF Core DbContext, migrations, and registration
  - **Store/** - Fluxor state management (actions, reducers, state)
  - **Extensions/** - Extension methods
  - **Migrations/** - EF Core migrations

### Key Technologies

- **.NET 10** - Target framework
- **Blazor Interactive Server** - Rendering mode
- **Fluxor** - State management (Redux pattern)
- **Radzen.Blazor** - UI component library
- **Entity Framework Core 10** - ORM (SQL Server)
- **FluentValidation** - Input validation
- **NLog** - Logging
- **FirebirdSql** - Legacy KRD data source
- **Blazilla** - Additional Blazor utilities

### State Management (Fluxor)

- All state is managed through Fluxor with the Redux pattern
- State files are in `Store/RepairOrderFeature/`
- Actions, Reducers, and State classes follow Fluxor conventions
- Redux DevTools are enabled in DEBUG mode

### Data Access

- **AppDbContext** - Main application database (SQLite or SQL Server)
- **KrdDbContext** - Legacy Firebird database for KRD data
- Both use factory pattern (`IAppDbContextFactory`, `IKrdDbContextFactory`)
- Migrations are applied automatically on startup via `dbContext.Database.MigrateAsync()`
- Masterdata is initialized on startup via `IMasterdataService.InitializeAsync()`

### Mapping

- Custom **Mapper** service (not AutoMapper) implements `IMapper`
- Uses a switch expression pattern for type-to-type mapping
- All mapping logic is in `Services/Mapper.cs`

### Validation

- **FluentValidation** validators are registered automatically via `AddValidatorsFromAssemblyContaining<Program>()`
- Validators follow the pattern: `{EntityName}VmValidator : AbstractValidator<{EntityName}Vm>`

### Domain Entities

All domain entities inherit from `AuditableEntity` which provides:

- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?)
- `ArchivedAt` (DateTime?)

Key entities: `RepairOrder`, `RepairPosition`, `Defective`, `IncludedComponent`, `Employee`, `LogisticProvider`, `RepairOrderStatus`, `RepairOrderStatusHistory`, `DataStore`

### Service Layer

- Services are registered as Scoped (per-circuit in Blazor Server)
- `IMasterdataService` is Singleton (initialized once at startup)
- Services follow the pattern: `I{EntityName}Service` / `{EntityName}Service`

## Coding Conventions

### Naming

- **PascalCase** for classes, methods, properties, namespaces
- **camelCase** for local variables and method parameters
- **Interfaces** prefixed with `I` (e.g., `IRepairOrderService`)
- **ViewModels** suffixed with `Vm` (e.g., `RepairOrderVm`)
- **Validators** suffixed with `Validator` (e.g., `RepairOrderVmValidator`)

### File Organization

- One class per file (except for small related types)
- Namespaces match folder structure
- Domain entities in `CamCare.Domain`
- ViewModels in `CamCare.Models`
- Services in `CamCare.Services`
- Interfaces in `CamCare.Interfaces.Services` or `CamCare.Interfaces.Persistence`

### Blazor Components

- Use `@rendermode="InteractiveServer"` for interactive components
- Radzen components are preferred for UI (Radzen.Blazor)
- Layout is defined in `Components/Layout/MainLayout.razor`
- Common imports are in `Components/_Imports.razor`

### Dependency Injection

- Constructor injection is used throughout
- Services are registered in `Program.cs` using `AddScoped`, `AddSingleton`, or `AddTransient`
- DbContext factories are registered via `AddPersistence()` extension method

### Database

- Use `DateTime.UtcNow` for storing dates in entities
- Convert to local time when displaying in ViewModels (`.ToLocalTime()`)
- Convert back to UTC when mapping back to entities (`.ToUniversalTime()`)
- Cascade delete for RepairOrder, Restrict delete for related entities

## Common Tasks

### Adding a New Entity

1. Create domain class in `Domain/` inheriting from `AuditableEntity`
2. Add DbSet to `AppDbContext`
3. Create ViewModel in `Models/`
4. Create validator in `Models/` (if needed)
5. Add mapping cases in `Services/Mapper.cs`
6. Create service interface in `Interfaces/Services/`
7. Create service implementation in `Services/`
8. Register service in `Program.cs`
9. Create EF Core migration
10. Create Blazor pages in `Components/Pages/`

### Adding a New Page

1. Create `.razor` file in `Components/Pages/`
2. Add navigation entry in `MainLayout.razor` sidebar
3. Use Radzen components for UI
4. Inject Fluxor state and services as needed

## Important Notes

- The application uses German UI text (labels, messages, etc.)
- All monetary or measurement values use German formatting conventions
- The app connects to an external Amicron API for customer/serial data
- Legacy KRD data is read-only from a Firebird database
- Docker support is configured for Linux containers
