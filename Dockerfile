FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia apenas o projeto para restaurar dependências
COPY ["app_pm_back.csproj", "./"]
RUN dotnet restore "app_pm_back.csproj"

# Copia TUDO (incluindo a pasta DTO)
COPY . .

# Publica direto (o publish já faz o build internamente)
RUN dotnet publish "app_pm_back.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "app_pm_back.dll"]