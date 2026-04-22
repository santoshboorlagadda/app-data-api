# proj.md — app-data-api / data-api

## Project Overview
.NET Web API providing employee-related endpoints backed by PostgreSQL via Entity Framework Core. This repository is upgraded to .NET 8 and latest stable NuGet dependencies compatible with .NET 8.

## Tech Stack
- Language: C# (.NET 8)
- Framework: ASP.NET Core Web API
- ORM: Entity Framework Core 8
- Database Provider: PostgreSQL (Npgsql)
- Auth: None (not present in current implementation)
- API Docs: Swagger / OpenAPI (Swashbuckle)
- Test Framework: Existing repo tests (if any) run via `dotnet test`
- Logging: Built-in ASP.NET Core logging (and any configured providers in code)

## Project Structure
- `src/data-api/` (main Web API project)
  - `Controllers/` (API controllers)
  - `Models/` (domain/DTO models)
  - `Data/` (EF Core DbContext)
  - `Program.cs` (minimal hosting)
  - `appsettings*.json` (configuration)
- Solution root contains solution file and optional supporting files (e.g., Dockerfile).

## Commands
- Restore: `dotnet restore`
- Build: `dotnet build`
- Test: `dotnet test`
- Run (dev): `dotnet run --project src/data-api`
- Publish: `dotnet publish -c Release`

## Database
- Provider: PostgreSQL
- Connection: Configured via `appsettings.json` / environment variables (typical `ConnectionStrings:*`)
- ORM: EF Core 8 + Npgsql EF Core provider

## Coding Conventions
- Keep existing API routes unchanged.
- Prefer async EF Core APIs.
- Use DI for DbContext and services.
- Avoid introducing new endpoints or behavior changes during upgrade.
- Ensure build succeeds.

## Agent Instructions
- Goal is framework/package upgrade + compatibility refactors only.
- Remove template sample artifacts: `WeatherForecastController` and `WeatherForecast` model if present.
- Upgrade all `.csproj` TargetFramework to `net8.0`.
- Upgrade all NuGet packages to latest stable versions compatible with .NET 8, including EF Core 8 and Npgsql-compatible versions.
- Do not add new tests; ensure existing endpoints continue to work.

## Do Not Touch
- Do not change public API routes or versioning structure.
- Do not introduce new authentication/authorization mechanisms.
- Do not modify infrastructure/CI (none required for this PR).
