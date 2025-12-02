# Etapa 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar los archivos del proyecto
COPY SentinelBackend.Api/SentinelBackend.Api.csproj SentinelBackend.Api/
RUN dotnet restore SentinelBackend.Api/SentinelBackend.Api.csproj

# Copiar todo el código
COPY . .

# Publicar en modo Release
RUN dotnet publish SentinelBackend.Api/SentinelBackend.Api.csproj -c Release -o /app/publish

# Etapa 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SentinelBackend.Api.dll"]
