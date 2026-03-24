# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY src/data-api/ ./src/data-api/
COPY src/certs/ ./src/certs/

RUN dotnet restore ./src/data-api/data-api.csproj
RUN dotnet publish ./src/data-api/data-api.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish/ ./
COPY --from=build /src/src/certs/ /app/certs/

ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "data-api.dll"]
