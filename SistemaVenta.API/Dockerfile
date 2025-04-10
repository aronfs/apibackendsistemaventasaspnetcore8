# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar solución y proyectos para restaurar dependencias
COPY *.sln ./
COPY SistemaVenta.API/*.csproj ./SistemaVenta.API/
COPY SistemaVenta.BLL/*.csproj ./SistemaVenta.BLL/
COPY SistemaVenta.DAL/*.csproj ./SistemaVenta.DAL/
COPY SistemaVenta.DTO/*.csproj ./SistemaVenta.DTO/
COPY SistemaVenta.IOC/*.csproj ./SistemaVenta.IOC/
COPY SistemaVenta.Model/*.csproj ./SistemaVenta.Model/
COPY SistemaVenta.Utility/*.csproj ./SistemaVenta.Utility/

# Restaurar dependencias
RUN dotnet restore

# Copiar todo el código
COPY . .

# Publicar la app
WORKDIR /app/SistemaVenta.API
RUN dotnet publish -c Release -o /app/out

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Exponer el puerto (debe coincidir con el configurado en Railway)
EXPOSE 5185

# Variables opcionales
ENV ASPNETCORE_URLS=http://+:5186
ENV ASPNETCORE_ENVIRONMENT=Production

# Copiar la app publicada
COPY --from=build /app/out ./

# Iniciar la app
ENTRYPOINT ["dotnet", "SistemaVenta.API.dll"]
