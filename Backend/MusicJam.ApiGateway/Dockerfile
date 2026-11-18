FROM node:22-alpine AS frontend-build
WORKDIR /frontend
COPY Frontend/package*.json ./
RUN npm ci
COPY Frontend/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Backend/MusicJam.ApiGateway/MusicJam.ApiGateway.csproj", "Backend/MusicJam.ApiGateway/"]
RUN dotnet restore "Backend/MusicJam.ApiGateway/MusicJam.ApiGateway.csproj"
COPY . .
WORKDIR "/src/Backend/MusicJam.ApiGateway"
RUN dotnet build "./MusicJam.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MusicJam.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=frontend-build /frontend/dist ./wwwroot
ENTRYPOINT ["dotnet", "MusicJam.ApiGateway.dll"]