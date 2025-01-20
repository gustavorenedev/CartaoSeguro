FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/CartaoSeguro.API/CartaoSeguro.API.csproj", "API/CartaoSeguro.API/"]
COPY ["Application/CartaoSeguro.Application/CartaoSeguro.Application.csproj", "Application/CartaoSeguro.Application/"]
COPY ["Core/CartaoSeguro.Core/CartaoSeguro.Domain.csproj", "Core/CartaoSeguro.Core/"]
COPY ["Infrastructure/CartaoSeguro.Infrastructure/CartaoSeguro.Infrastructure.csproj", "Infrastructure/CartaoSeguro.Infrastructure/"]
RUN dotnet restore "./API/CartaoSeguro.API/CartaoSeguro.API.csproj"
COPY . .
WORKDIR "/src/API/CartaoSeguro.API"
RUN dotnet build "./CartaoSeguro.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CartaoSeguro.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartaoSeguro.API.dll"]