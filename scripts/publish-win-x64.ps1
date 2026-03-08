param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..")
$project = Join-Path $repoRoot "src/RowsLink.App/RowsLink.App.csproj"

if (-not (Test-Path $project)) {
    throw "Project file not found: $project"
}

Write-Host "Using project: $project"

# Run from repository root to keep all relative paths stable.
Push-Location $repoRoot
try {
    dotnet publish $project `
      -c $Configuration `
      -r win-x64 `
      --self-contained true `
      /p:PublishSingleFile=true
}
finally {
    Pop-Location
}

Write-Host "Built EXE: $repoRoot/src/RowsLink.App/bin/$Configuration/net9.0-windows/win-x64/publish/RowsLink.exe"
