# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./

COPY SistemaVenta.API/SistemaVenta.API.csproj SistemaVenta.API/
COPY SistemaVenta.BLL/SistemaVenta.BLL.csproj SistemaVenta.BLL/
COPY SistemaVenta.DAL/SistemaVenta.DAL.csproj SistemaVenta.DAL/
COPY SistemaVenta.DTO/SistemaVenta.DTO.csproj SistemaVenta.DTO/
COPY SistemaVenta.IOC/SistemaVenta.IOC.csproj SistemaVenta.IOC/
COPY SistemaVenta.Model/SistemaVenta.Model.csproj SistemaVenta.Model/
COPY SistemaVenta.Utility/SistemaVenta.Utility.csproj SistemaVenta.Utility/

RUN dotnet restore

COPY . .

WORKDIR /src/SistemaVenta.API
RUN dotnet publish -c Release -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5185

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SistemaVenta.API.dll"]
