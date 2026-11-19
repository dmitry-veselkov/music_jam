FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Backend/MusicJam.Studio/MusicJam.Studio.csproj", "Backend/MusicJam.Studio/"]
COPY ["Backend/MusicJam.Shared/MusicJam.Shared.csproj", "Backend/MusicJam.Shared/"]
RUN dotnet restore "Backend/MusicJam.Studio/MusicJam.Studio.csproj"
COPY . .
WORKDIR "/src/Backend/MusicJam.Studio"
RUN dotnet build "./MusicJam.Studio.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MusicJam.Studio.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicJam.Studio.dll"]