param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$project = "src/RowsLink.App/RowsLink.App.csproj"

dotnet publish $project `
  -c $Configuration `
  -r win-x64 `
  --self-contained true `
  /p:PublishSingleFile=true

Write-Host "Built EXE: src/RowsLink.App/bin/$Configuration/net8.0-windows/win-x64/publish/RowsLink.exe"
