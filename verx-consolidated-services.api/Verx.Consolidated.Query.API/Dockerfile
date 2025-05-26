FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

ENV PORT=8083
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["nuget-local", "Verx.Consolidated.Query.API/nuget-local"]
COPY ["NuGet.Config", "Verx.Consolidated.Query.API/NuGet.Config"]
COPY ["Verx.Consolidated.Query.API/Verx.Consolidated.Query.Api.csproj", "Verx.Consolidated.Query.API/"]

RUN dotnet restore "Verx.Consolidated.Query.API/Verx.Consolidated.Query.Api.csproj"

COPY . .

WORKDIR "/src/Verx.Consolidated.Query.API"
RUN dotnet build "Verx.Consolidated.Query.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Verx.Consolidated.Query.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Verx.Consolidated.Query.Api.dll"]