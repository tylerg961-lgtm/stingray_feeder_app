# StingrayFeeder — Razor Pages Stingray Feeding & Fish Monitoring (target: .NET 9)

StingrayFeeder is a focused Razor Pages web application (targeting `.NET 9`) for managing feeding operations for captive stingrays and monitoring the fish that are fed to them. 
The app demonstrates domain modeling for animals and feed, separation of concerns via DI, full CRUD for feeding records and fish inventory, diagnostics and health checks for reliable 
operations, structured logging (including correlation IDs for feeding sessions), and a compact stored-procedure report for recent feeding statistics. The project is scoped so each 
week implements one clear concept and produces testable, reviewable deliverables suitable for GitHub submission.

## Migration & Evidence (Week 10 deliverable)

This repository now includes the minimal domain model and a corresponding initial EF Core migration to support upcoming Razor Pages work.

### What I added
- Lightweight POCO entity classes: `Stingray`, `Fish`, `FeedBatch`, `Caretaker`. These complement the already-present `FeedEvent` and match the `DbSet<T>` entries exposed by `ApplicationDbContext`.
- An EF Core migration file `Migrations/20251027223400_InitialCreate.cs` that creates the tables and foreign key relationships required for feed tracking.

### Why this is minimal
The goal was to provide a stable, normalized surface for CRUD pages and service logic in later weeks. `FeedBatch` separates inventory (batches of fish/feed) from `FeedEvent` which records consumption; this keeps historical usage and inventory separate and simplifies reporting & auditing.

### How to reproduce locally
1. Ensure packages are installed: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design`. Optionally install `dotnet-ef` globally.
2. From the project folder containing `ApplicationDbContext` run:
   - `dotnet ef migrations add InitialCreate --context ApplicationDbContext`
   - `dotnet ef database update --context ApplicationDbContext`
If you already have the migration file in `Migrations/` you can skip the first step and run only `dotnet ef database update`.

### Evidence to include in your GitHub submission
- Commit containing the model classes, `ApplicationDbContext` registration, and `Migrations/*`. Example commit message: "Add minimal domain models and initial EF migration".
- Artifacts directory with the following screenshots:
  - Terminal output showing `dotnet ef migrations add InitialCreate` (or PMC output).
  - Terminal output showing `dotnet ef database update`.
  - SQL Server Object Explorer / DB browser screenshot showing created tables (`Fish`, `FeedBatches`, `Stingrays`, `Caretakers`, `FeedEvents`).
- A brief explanation (this section) explaining why the model is shaped this way and what the migration created.

### Next steps
- Add seed data (in `DbInitializer` or `DbContext.OnModelCreating`) if you want reproducible demo data.
- Scaffold Razor Pages for `FeedEvent` and `Fish` to demonstrate CRUD and inventory behavior.

## Planning table (Weeks 10–15)

| Week | Concept | Feature | Goal / Objective | Done checklist | Documentation / Evidence (what I will provide) | How I'll test |
|---:|---|---|---|---|---|---|
| Week 10 | Modeling | Domain model and `AppDbContext` for `Stingray`, `Fish`, `FeedEvent`, `FeedBatch` | Create the core domain: represent stingrays, fish inventory (species, size, source), feed events linking stingrays to fish batches and timestamps. Ensure relationships, constraints, and basic indexes so data supports monitoring and reporting. | - POCOs: `Stingray`, `Fish`, `FeedEvent`, `FeedBatch` with navigation props<br/>- `Data/AppDbContext.cs` with `DbSet<>`s and fluent mappings where needed<br/>- Initial EF Core migration and local DB seed script | Code committed to GitHub: model classes, `AppDbContext`, migration files, a SQL schema snapshot. I will add a 200+ word write-up in this `README.md` describing modeling decisions (why separate `FeedBatch` from `FeedEvent`, normalization, key choices) and include an ER diagram or PlantUML screenshot. | - Run `dotnet ef migrations add Initial` and `dotnet ef database update` against SQLite or LocalDB and inspect schema.<br/>- Unit tests verifying required fields, FK behavior, and expected navigation loading. |
| Week 11 | Separation of Concerns / Dependency Injection | Repository and service layers for feeding operations and fish inventory | Keep UI thin: `Pages` depend on service interfaces not EF Core directly. Provide `IFeedService`, `IFishRepository` to encapsulate business rules (e.g., decrement inventory when used). Register services in DI with appropriate lifetimes. | - Define interfaces (`IFeedService`, `IFishRepository`) and concrete implementations<br/>- Register services in `Program.cs` with `builder.Services.AddScoped<>`<br/>- Unit tests using mocks to validate service logic (inventory decrements, validation) | Commit interfaces, implementations, and updated `Program.cs`. README will include a 200+ word write-up explaining DI choices, lifetimes (`Scoped` for DbContext services), and a small diagram showing layers. | - Run unit tests with mocked repositories (Moq) to verify behavior without DB.<br/>- Start the app and confirm pages receive services via constructor injection. |
| Week 12 | CRUD | Razor Pages CRUD for `FeedEvent` and `Fish` with validation and inventory rules | Provide full Create/Read/Update/Delete flows: add fish to inventory, log feed events (select stingray + fish batch + quantity), validate available inventory, show feeding history per stingray. | - Razor Pages: `Pages/Fish/*`, `Pages/FeedEvents/*`, `Pages/Stingrays/*` (`Index`, `Details`, `Create`, `Edit`, `Delete`)<br/>- Server-side validation and antiforgery protection<br/>- Integration tests using in-memory DB provider | Commit all Razor Pages and `PageModel` code. README will include a 200+ word feature write-up and screenshots showing create form, validation, and feeding history per animal. | - Manual tests: create fish entries, perform feed event (inventory reduces), edit/delete flows.<br/>- Integration tests against in-memory provider asserting CRUD and inventory integrity. |
| Week 13 | Diagnostics | Health checks, metrics for recent feed rates, request timing middleware | Add health checks and a diagnostics page showing recent feed throughput, average feed session durations, and recent errors to help caretakers and maintainers quickly assess system health. | - Configure `Microsoft.Extensions.Diagnostics.HealthChecks` and `/health` endpoint<br/>- Add middleware capturing request durations and feed-operation latencies<br/>- Add `/Diagnostics` Razor Page summarizing recent metrics | Commit health check config, middleware, and diagnostics page. README will include a 200+ word write-up describing diagnostic metrics, endpoint examples, and screenshots of the diagnostics page. | - Call `/health` to validate healthy/unhealthy toggles.<br/>- Generate feed events and verify metrics appear on `/Diagnostics`.<br/>- Tests for middleware timing logic. |
| Week 14 | Logging | Structured logging (Serilog), correlation IDs per feeding session, and log enrichment | Implement structured logs for traceability: every feed event gets a correlation ID; logs include `StingrayId`, `FishId`, `FeedBatchId`, and user/caretaker where available. Export logs to file and optionally Seq. | - Serilog bootstrap in `Program.cs` and `appsettings.json` sinks<br/>- Correlation ID middleware that attaches `X-Correlation-ID` to feed requests<br/>- Ensure key operations log structured JSON entries | Commit logging configuration, middleware, and example log files (JSON). README will include a 200+ word write-up with sample log entries and explanation of retention and search strategies. | - Perform feed events and verify logs contain structured fields and correlation ID.<br/>- Search sample log file for event types (create/edit/delete feed). |
| Week 15 | Stored Procedures | Stored-procedure report `GetRecentFeedSummaryByStingray` invoked from EF Core or Dapper | Provide a lightweight stored-procedure-backed report that returns recent feeding stats per stingray (last 7 days): items fed, total mass, last fed timestamp. Keep scope small due to holiday week. | - SQL script to create `GetRecentFeedSummaryByStingray` (read-only)<br/>- Repository method calling the proc (`FromSqlRaw` or Dapper) and `Pages/Reports/RecentFeed` page<br/>- Integration test executing proc against test DB | Commit SQL script, repository proc call, report page, and a 200+ word README write-up explaining trade-offs for stored procs in reporting scenarios with a sample output screenshot. | - Run SQL script to create proc, seed data, visit `/Reports/RecentFeed` and validate results.<br/>- Integration test asserting expected aggregated values from the stored proc. |

## Week 11 — Separation of Concerns & Dependency Injection

For Week 11 I implemented a small service layer to move non-UI business logic out of controllers and into a testable, injectable service. The new interface and implementation are `Services/IFeedingService.cs` and `Services/FeedingService.cs`. The service encapsulates business rules for recording feed events (applying sensible defaults such as assigning a correlation id and UTC event time) and querying recent feed events.

### Registration and lifetime

- The service is registered in `Program.cs` using `builder.Services.AddScoped<IFeedingService, FeedingService>();`.
- `Scoped` is chosen intentionally because `FeedingService` depends on `AppDbContext`, which is registered as scoped; matching lifetimes avoids capturing a shorter-lived dependency from a longer-lived service and is the recommended pattern for services that use EF Core.

### Controller usage

- Controllers (and Razor Page models) are kept thin. `Controllers/FeederController.cs` (the API controller) receives `IFeedingService` via constructor injection and delegates work to it in both the `GetRecent` (reads recent feed events) and `Post` (records a new feed event) actions.
- The service performs defaults, persistence, and logging; the controller only validates the request and returns appropriate HTTP responses.

### How to validate locally (evidence)

1. Run the app: `dotnet run` from the project folder.
2. Ensure DB is migrated (the app applies `Database.Migrate()` in development).
3. Test the read endpoint:
   - `curl "http://localhost:5000/api/feeding/recent?max=5"`
4. Test the post endpoint:
   - `curl -X POST -H "Content-Type: application/json" -d "{\"StingrayId\":1,\"FishId\":1,\"Quantity\":3}" http://localhost:5000/api/feeding`
5. Capture screenshots of terminal output, the JSON response, and the database (SQL Server Object Explorer) showing the newly added `FeedEvents` row — include these in your GitHub submission as evidence.

### Design notes

- Keeping business rules in a service improves testability (unit tests can mock `IFeedingService` or test `FeedingService` with an in-memory or test database), reduces duplication across pages/controllers, and makes it straightforward to add cross-cutting behaviors (metrics, retry, validation) in one place.

### Files changed/added

- `Services/IFeedingService.cs` (interface)
- `Services/FeedingService.cs` (implementation)
- `Program.cs` (service registration)
- `Controllers/FeederController.cs` (uses the service)

This section documents the feature and should be committed alongside screenshots and logs as described above.

## Deliverables and workflow notes
- The project will target `.NET 9` and use `Razor Pages` patterns. Key paths: `Program.cs`, `appsettings.json`, `Pages/*`, `Data/AppDbContext.cs`, `Services/*`, and `Migrations/*`.
- Each weekly feature will be developed in feature branches (`week10-modeling`, `week11-di`, etc.) and merged to `main` after tests pass.
- For each completed feature I will provide:
  - Source code in the repository (models, services, pages, middleware, tests).
  - A 200+ word write-up in this `README.md` for that feature describing design decisions, trade-offs, and evidence (plus screenshots, logs, or ER diagrams as appropriate).
  - Automated tests (unit / integration) demonstrating correctness.


