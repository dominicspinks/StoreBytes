FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN mkdir -p /file-storage/StoreBytes.API && chown -R $APP_UID:$APP_UID /file-storage
RUN mkdir -p /https && chown -R $APP_UID:$APP_UID /https
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StoreBytes.API/StoreBytes.API.csproj", "StoreBytes.API/"]
COPY ["StoreBytesLibrary/StoreBytesLibrary.csproj", "StoreBytesLibrary/"]
RUN dotnet restore "./StoreBytes.API/StoreBytes.API.csproj"
COPY . .
WORKDIR "/src/StoreBytes.API"
RUN dotnet build "./StoreBytes.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StoreBytes.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreBytes.API.dll"]