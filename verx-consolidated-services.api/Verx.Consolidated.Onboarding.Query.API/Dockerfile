#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

ENV PORT=8083
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Verx.Consolidated.Onboarding.Query.API/Verx.Consolidated.Onboarding.Query.Api.csproj", "Verx.Consolidated.Onboarding.Query.API/"]
RUN dotnet restore "Verx.Consolidated.Onboarding.Query.API/Verx.Consolidated.Onboarding.Query.Api.csproj"
COPY . .
WORKDIR "/src/Verx.Consolidated.Onboarding.Query.API"
RUN dotnet build "Verx.Consolidated.Onboarding.Query.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Verx.Consolidated.Onboarding.Query.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Verx.Consolidated.Onboarding.Query.Api.dll"]