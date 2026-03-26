# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore DataApi.sln
RUN dotnet publish src/data-api/DataApi.csproj -c Release -o /app/publish

# Run
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "DataApi.dll"]
