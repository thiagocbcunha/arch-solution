FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

RUN ls -la

COPY ["nuget-local", "Verx.Consolidated.Worker/nuget-local"]

RUN ls -la

COPY ["NuGet.Config", "Verx.Consolidated.Worker/NuGet.Config"]
COPY ["Verx.Consolidated.Worker/Verx.Consolidated.Worker.csproj", "Verx.Consolidated.Worker/"]

RUN dotnet restore "./Verx.Consolidated.Worker\Verx.Consolidated.Worker.csproj"

COPY . .

WORKDIR "/src/Verx.Consolidated.Worker"
RUN dotnet build "./Verx.Consolidated.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Verx.Consolidated.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Verx.Consolidated.Worker.dll"]