# app-data-api

## Requirements
- .NET SDK 8.x

## Build
```bash
dotnet restore
dotnet build
```

## Test
```bash
dotnet test
```

## Run
```bash
dotnet run --project src/data-api/DataApi.csproj
```

## Entity Framework (tooling)
If you use EF Core CLI tooling locally:
```bash
dotnet tool install --global dotnet-ef --version 8.*
# or
# dotnet tool update --global dotnet-ef --version 8.*
```
