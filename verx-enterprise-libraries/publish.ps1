# Caminho base (diretório onde o script está)
$BASE_DIR = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Diretório onde os pacotes .nupkg serão salvos
$OUTPUT_DIR = Join-Path $BASE_DIR "nuget-packages"
New-Item -ItemType Directory -Path $OUTPUT_DIR -Force | Out-Null

# Pastas adicionais para replicar os pacotes
$REPLICA_DIRS = @(
    (Join-Path $BASE_DIR "../verx-authentication-service.api/nuget-local")
    (Join-Path $BASE_DIR "../verx-transaction-flow.serveless/nuget-local")
    (Join-Path $BASE_DIR "../verx-consolidated-services.api/nuget-local")
)
foreach ($dir in $REPLICA_DIRS) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
}

# Encontrar todos os arquivos .csproj em subdiretórios
Get-ChildItem -Path $BASE_DIR -Recurse -Filter *.csproj | ForEach-Object {
    $csproj = $_.FullName
    Write-Host "Publicando $csproj em modo Release..."
    dotnet pack $csproj -c Release -o $OUTPUT_DIR
}

# Replicar cada pacote gerado nas 3 pastas
Get-ChildItem -Path $OUTPUT_DIR -Filter *.nupkg | ForEach-Object {
    foreach ($dir in $REPLICA_DIRS) {
        Copy-Item $_.FullName -Destination $dir -Force
    }
}

Write-Host "Pacotes NuGet gerados em: $OUTPUT_DIR e replicados em: $($REPLICA_DIRS -join ', ')"

if (Test-Path $OUTPUT_DIR) {
    Remove-Item -Path $OUTPUT_DIR -Recurse -Force
    Write-Host "Diretório $OUTPUT_DIR removido."
}