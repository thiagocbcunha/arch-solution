FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /src/verx-enterprise-libraries

RUN mkdir -p /src/nuget-local

COPY . .

CMD for csproj in $(find . -name '*.csproj'); do \
      echo "Processando projeto: $csproj"; \
      dotnet pack "$csproj" -c Release -o /tmp/nugets --configfile /src/verx-enterprise-libraries/NuGet.Config; \
      cp /tmp/nugets/*.nupkg /src/nuget-local/; \
    done