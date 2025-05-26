#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

ENV PORT=8085
EXPOSE 8085

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["nuget-local", "Verx.Consolidated.Command.API/nuget-local"]
COPY ["NuGet.Config", "Verx.Consolidated.Command.API/NuGet.Config"]
COPY ["Verx.Consolidated.Command.API/Verx.Consolidated.Command.Api.csproj", "Verx.Consolidated.Command.API/"]

RUN dotnet restore "Verx.Consolidated.Command.API/Verx.Consolidated.Command.Api.csproj"

COPY . .

WORKDIR "/src/Verx.Consolidated.Command.API"
RUN dotnet build "Verx.Consolidated.Command.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Verx.Consolidated.Command.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Verx.Consolidated.Command.Api.dll"]