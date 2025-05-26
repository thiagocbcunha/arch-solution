FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["nuget-local", "Verx.TransactionFlow.Serveless/nuget-local"]
COPY ["NuGet.Config", "Verx.TransactionFlow.Serveless/NuGet.Config"]
COPY ["Verx.TransactionFlow.Serveless/Verx.TransactionFlow.Serveless.csproj", "Verx.TransactionFlow.Serveless/"]

RUN dotnet restore "Verx.TransactionFlow.Serveless/Verx.TransactionFlow.Serveless.csproj"

COPY . .

WORKDIR "/src/Verx.TransactionFlow.Serveless"
RUN dotnet build "Verx.TransactionFlow.Serveless.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Verx.TransactionFlow.Serveless.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV PORT=8082
EXPOSE 8082

ENTRYPOINT ["dotnet", "Verx.TransactionFlow.Serveless.dll"]