# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución
COPY *.sln ./

# Copiar proyectos individualmente (ajustando ruta real)
COPY SistemaVenta.API/SistemaVenta.API.csproj SistemaVenta.API/
COPY SistemaVenta.BLL/SistemaVenta.BLL.csproj SistemaVenta.BLL/
COPY SistemaVenta.DAL/SistemaVenta.DAL.csproj SistemaVenta.DAL/
COPY SistemaVenta.DTO/SistemaVenta.DTO.csproj SistemaVenta.DTO/
COPY SistemaVenta.IOC/SistemaVenta.IOC.csproj SistemaVenta.IOC/
COPY SistemaVenta.Model/SistemaVenta.Model.csproj SistemaVenta.Model/
COPY SistemaVenta.Utility/SistemaVenta.Utility.csproj SistemaVenta.Utility/

# Restaurar dependencias
RUN dotnet restore

# Copiar el resto del código
COPY . .

# Publicar aplicación
WORKDIR /src/SistemaVenta.API
RUN dotnet publish -c Release -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Puerto de exposición
EXPOSE 5185

# Copiar archivos publicados
COPY --from=build /app/publish .

# Comando de inicio
ENTRYPOINT ["dotnet", "SistemaVenta.API.dll"]
