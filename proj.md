# proj.md — DataApi (data-api)

## Project Overview
DataApi is a .NET 6 Web API that exposes REST endpoints to read employee data from a PostgreSQL database (`mysample`) table `public.employees`. Primary endpoint: `GET /api/v1/employees` with pagination.

Repo: https://github.com/santoshboorlagadda/app-data-api  
Target branch: `develop`

## Tech Stack
- Language: C# (.NET 6)
- Framework: ASP.NET Core Web API (Controllers)
- ORM: Entity Framework Core (Npgsql provider), **DB-first scaffolding**
- Auth: none
- Test Framework: MSTest
- Mocking: Moq (allowed)
- API Docs: Swagger/OpenAPI (enabled in all environments)
- Logging: Serilog (JSON-formatted console logs)
- Error Handling: Global exception handler returning custom error JSON

## Project Structure
- `src/data-api/` — Web API project (`DataApi`)
  - `Controllers/`
  - `Models/` (scaffolded EF entities)
  - `Data/` (DbContext)
  - `Repositories/`
  - `Services/`
  - `Middleware/` (global exception handling)
  - `appsettings.json`
- `tests/DataApi.Tests/` — MSTest unit tests (controller, service, repository)
- `Dockerfile` — container build for Docker deployment

## Commands
From repo root:
- Create solution (if needed): `dotnet new sln -n DataApi`
- Create API project: `dotnet new webapi -n DataApi -o src/data-api`
- Add to solution: `dotnet sln add src/data-api/DataApi.csproj`
- Restore: `dotnet restore`
- Build: `dotnet build`
- Test: `dotnet test`
- Run (dev): `dotnet run --project src/data-api/DataApi.csproj`

### EF Core DB-First Scaffolding (PostgreSQL)
Scaffold `public.employees` from DB `mysample` (do not commit secrets in the scaffold command):
- `dotnet tool install --global dotnet-ef --version 6.*` (if needed)
- Example scaffold (adjust host/user/pass in command):
  - `dotnet ef dbcontext scaffold "Host=mydb.aws.com;Port=5432;Database=mysample;Username=postgres;Password=<REDACTED>;SslMode=Require" Npgsql.EntityFrameworkCore.PostgreSQL --project src/data-api/DataApi.csproj --output-dir Models --context-dir Data --context AppDbContext --schema public --table employees --use-database-names --no-onconfiguring --force`

Notes:
- Connection string is stored in `src/data-api/appsettings.json` (per requirement). Do not commit passwords; use env vars/user-secrets.
- Migrations are not planned (schema is managed externally / DB-first).

## Database
- Provider: PostgreSQL
- Host/Port: `mydb.aws.com:5432`
- Database: `mysample`
- Schema/Table: `public.employees`
- ORM: EF Core (Npgsql)
- Entity shape (confirmed):
  - `emp_id` INT, PK, DB-generated identity
  - `emp_name` VARCHAR(255), NULL

## API Conventions
- Base route: `/api/v1`
- Endpoint:
  - `GET /api/v1/employees?page=1&pageSize=100`
- Sorting: `emp_id` ascending
- Pagination:
  - Default: `page=1`, `pageSize=100`
  - Invalid `page`/`pageSize` ⇒ `400 Bad Request`
- Response shape:
  ```json
  { "items": [], "page": 1, "pageSize": 100, "totalCount": 0, "totalPages": 0 }
  ```
- Empty result: `200 OK` with `items: []`

## Error Handling
- Global exception handler middleware returns:
  ```json
  { "error": { "code": "...", "message": "...", "traceId": "..." } }
  ```
- Also uses controller-level try/catch where explicitly implemented (per requirement), but global handler is authoritative for unhandled exceptions.

## Dependency Injection
- DbContext registered with Npgsql provider.
- Repository + service pattern:
  - `IEmployeeRepository` / `EmployeeRepository`
  - `IEmployeeService` / `EmployeeService`
- Controllers depend on service interfaces.

## Logging
- Serilog configured to write **JSON** to console.
- Include correlation via `traceId` (from `HttpContext.TraceIdentifier`) in error responses.

## Docker
- Dockerfile builds and runs the API for Docker deployment.
- Ports:
  - HTTP: 5000
  - HTTPS: 5001 (note: HTTPS certificate configuration must be provided separately if HTTPS is enabled in-container)

## Agent Instructions
- Do not add authentication/authorization.
- Do not introduce CQRS/mediator frameworks.
- Keep endpoint contract and pagination response shape exact.
- Use DB-first scaffolding approach for models.
