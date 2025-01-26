FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notifier/CartaoSeguro.Notifier/CartaoSeguro.Notifier.csproj", "Notifier/CartaoSeguro.Notifier/"]
COPY ["Domain/CartaoSeguro.Domain/CartaoSeguro.Domain.csproj", "Domain/CartaoSeguro.Domain/"]
RUN dotnet restore "./Notifier/WebApplication1/CartaoSeguro.Notifier.csproj"
COPY . .
WORKDIR "/src/Notifier/WebApplication1"
RUN dotnet build "./CartaoSeguro.Notifier.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CartaoSeguro.Notifier.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartaoSeguro.Notifier.dll"]