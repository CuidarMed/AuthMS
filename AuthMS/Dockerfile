# Etapa base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Etapa build (compilación)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY ["AuthMS/AuthMS.csproj", "AuthMS/"]
RUN dotnet restore "AuthMS/AuthMS.csproj"

# Copiar todo el código y compilar
COPY . .
WORKDIR "/src/AuthMS"
RUN dotnet build "AuthMS.csproj" -c Release -o /app/build

# Etapa de publicación
FROM build AS publish
RUN dotnet publish "AuthMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final (runtime)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthMS.dll"]
