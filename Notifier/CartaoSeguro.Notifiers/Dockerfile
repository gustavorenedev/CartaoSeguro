# Imagem base com ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notifier/CartaoSeguro.Notifiers/CartaoSeguro.Notifiers.csproj", "Notifier/CartaoSeguro.Notifiers/"]
RUN dotnet restore "./Notifier/CartaoSeguro.Notifiers/CartaoSeguro.Notifiers.csproj"
COPY . .
WORKDIR "/src/Notifier/CartaoSeguro.Notifiers"
RUN dotnet build "./CartaoSeguro.Notifiers.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Estágio de publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CartaoSeguro.Notifiers.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Estágio final com ASP.NET Core Runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartaoSeguro.Notifiers.dll"]
