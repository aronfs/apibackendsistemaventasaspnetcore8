# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar
COPY *.sln ./
COPY SistemaVenta.API/*.csproj ./SistemaVenta.API/
COPY SistemaVenta.BLL/*.csproj ./SistemaVenta.BLL/
COPY SistemaVenta.DAL/*.csproj ./SistemaVenta.DAL/
COPY SistemaVenta.DTO/*.csproj ./SistemaVenta.DTO/
COPY SistemaVenta.IOC/*.csproj ./SistemaVenta.IOC/
COPY SistemaVenta.Model/*.csproj ./SistemaVenta.Model/
COPY SistemaVenta.Utility/*.csproj ./SistemaVenta.Utility/
RUN dotnet restore

# Copiar todo el código
COPY . .

# Publicar la app
WORKDIR /app/SistemaVenta.API
RUN dotnet publish -c Release -o /out

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "SistemaVenta.API.dll"]
