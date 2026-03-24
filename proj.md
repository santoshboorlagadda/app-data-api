# proj.md — data-api

## Project Overview
`data-api` is a .NET 6 ASP.NET Core Web API that exposes employee data from a PostgreSQL database (`mysample`) via REST. Primary endpoint: `GET /api/v1/employees` with paging and filtering.

Repo: https://github.com/santoshboorlagadda/app-data-api  
Target branch: `develop`

## Tech Stack
- Language: C# (.NET 6 SDK)
- Framework: ASP.NET Core Web API (Controllers)
- ORM: Entity Framework Core (DB-first scaffolding)
- Database Provider: PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`
- Auth: None
- Test Framework: MSTest
- Mocking: Moq (allowed)
- API Docs: Swagger/OpenAPI (enabled in all environments)
- Logging: Serilog (JSON-formatted console logs)
- Error Handling: Global exception handler + minimal controller-level try/catch where needed

## Project Structure
- `src/data-api/` — main Web API project (single project)
  - `Controllers/`
  - `Models/` (scaffolded entity models)
  - `Data/` (scaffolded `MysampleContext`)
  - `Repositories/` (repository interfaces/impl)
  - `Services/` (service interfaces/impl)
  - `Middleware/` (global exception handler)
  - `certs/` (Postgres SSL certs copied into container image)
    - `root.crt`
    - `ca.crt`
- `tests/` — MSTest unit test project(s)

## Commands
- Restore: `dotnet restore`
- Build: `dotnet build`
- Test: `dotnet test`
- Run (local): `dotnet run --project src/data-api/data-api.csproj`

## Database
- Provider: PostgreSQL
- Database: `mysample`
- Table: `public.employees`
- Schema (exact): columns are **exactly**:
  - `emp_id` INT, Primary Key
  - `emp_name` VARCHAR(255), NULL
- Connection management: `appsettings.json` only (no user-secrets/env-var requirement stated)
- SSL:
  - `SslMode=Require`
  - Cert files: `src/certs/root.crt`, `src/certs/ca.crt` copied into Docker image at build time
- Migration strategy: None (DB-first; scaffolding required). Scaffolded `Employee` + `MysampleContext` are committed to the repo.

## API Contract
- Base route: `/api/v1`
- Endpoint: `GET /api/v1/employees`
  - Query:
    - `page` (default 1; must be > 0 else 400)
    - `pageSize` (default 100; must be > 0 else 400)
    - `empId` (optional, exact match)
    - `empname` (optional, **case-insensitive starts-with**)
  - Filter combination: `empId` and `empname` are combined with **OR** (both do not need to match).
  - Sorting: always `emp_id ASC` (no sort params)
  - Response (always JSON, 200 even when no rows):
    ```json
    {
      "items": [],
      "page": 1,
      "pageSize": 100,
      "totalCount": 0,
      "totalPages": 0
    }
    ```
  - `totalCount`: count after applying filters
  - `totalPages`: `ceil(totalCount / pageSize)`

## Docker
- Dockerfile required (only Dockerfile; no docker-compose requested)
- Exposed/listening ports:
  - HTTP: 5000
  - HTTPS: 5001
- Postgres SSL certs are copied into the image during build from `src/certs/`.

## Coding Conventions
- Async all the way for DB calls (`ToListAsync`, etc.)
- DI everywhere; no `new` for repositories/services in controllers
- Repository + Service pattern:
  - Repositories encapsulate EF querying
  - Services implement business/contract logic (paging, envelope, validation)
- JSON serialization: default `System.Text.Json` with camelCase properties (ensure envelope matches required casing)
- Logging:
  - Use Serilog with JSON console formatter
  - Log exceptions in global middleware with request context
- Error handling:
  - Return 400 for invalid paging inputs
  - Global exception handler returns custom JSON error payload (format defined in code, consistent across API)

## Agent Instructions
- Do not add auth/CQRS/testcontainers/integration tests.
- Use DB-first scaffolding outputs (`Employee`, `MysampleContext`) and commit them.
- Keep the solution to a single API project under `src/data-api/` plus tests under `tests/`.
- Swagger must be enabled in all environments.
- Ensure endpoint path and response envelope match the contract exactly.

## Do Not Touch
- N/A (repo is new/empty). Once created, avoid changing generated scaffolded files except via re-scaffold decision.

## Pattern References
- N/A (new repository)