# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar todo de una sola vez
COPY . .

# Restaurar dependencias
RUN dotnet restore

# Publicar la aplicación
WORKDIR /app/SistemaVenta.API
RUN dotnet publish -c Release -o /out

# Imagen final optimizada
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 5186

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /out .

ENTRYPOINT ["dotnet", "SistemaVenta.API.dll"]
