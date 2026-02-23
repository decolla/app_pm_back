FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["app_apm_back.csproj", "./"]
RUN dotnet restore "app_pm_back.csproj"

# Copia o restante dos arquivos e compila
COPY . .
RUN dotnet build "app_pm_back.csproj" -c Release -o /app/build

# Estágio 2: Publicação
FROM build AS publish
RUN dotnet publish "app_pm_back.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 3: Runtime final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5234

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "app_pm_back.dll"]