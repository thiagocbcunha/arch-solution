FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Verx.Authentication.Service.Api/Verx.Authentication.Service.Api.csproj", "Verx.Authentication.Service.Api/"]
RUN dotnet restore "Verx.Authentication.Service.Api/Verx.Authentication.Service.Api.csproj"
COPY . .
WORKDIR "/src/Verx.Authentication.Service.Api"
RUN dotnet build "Verx.Authentication.Service.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Verx.Authentication.Service.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Verx.Authentication.Service.Api.dll"]