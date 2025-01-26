FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8082
EXPOSE 8083


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notifiers/CartaoSeguro.Notifiers/CartaoSeguro.Notifiers.csproj", "Notifiers/CartaoSeguro.Notifiers/"]
RUN dotnet restore "./Notifiers/CartaoSeguro.Notifiers/CartaoSeguro.Notifiers.csproj"
COPY . .
WORKDIR "/src/Notifiers/CartaoSeguro.Notifiers"
RUN dotnet build "./CartaoSeguro.Notifiers.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CartaoSeguro.Notifiers.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartaoSeguro.Notifiers.dll"]