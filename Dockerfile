# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de solución y proyectos para aprovechar cache de restore
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

# Copiar el resto del código fuente
COPY . .

# Publicar la aplicación
WORKDIR /app/SistemaVenta.API
RUN dotnet publish -c Release -o /out

# Imagen final optimizada
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Puerto donde corre la app
EXPOSE 5185

# Opcional: establecer entorno a producción (útil para logs/configs)
ENV ASPNETCORE_ENVIRONMENT=Production

# Copiar archivos publicados desde etapa de build
COPY --from=build /out .

# Comando de inicio
ENTRYPOINT ["dotnet", "SistemaVenta.API.dll"]

