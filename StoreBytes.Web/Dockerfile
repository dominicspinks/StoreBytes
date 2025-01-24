FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN mkdir -p /https && chown -R $APP_UID:$APP_UID /https
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StoreBytes.Web/StoreBytes.Web.csproj", "StoreBytes.Web/"]
COPY ["StoreBytes.Common/StoreBytes.Common.csproj", "StoreBytes.Common/"]
RUN dotnet restore "./StoreBytes.Web/StoreBytes.Web.csproj"
COPY . .
WORKDIR "/src/StoreBytes.Web"
RUN dotnet build "./StoreBytes.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StoreBytes.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreBytes.Web.dll"]