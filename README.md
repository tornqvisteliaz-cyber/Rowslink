# RowsLink Windows App (WinForms, EXE-ready)

Detta repo innehåller en riktig **Windows app** (WinForms) i C#/.NET för RowsLink.

## Appfunktioner

- Modern Windows GUI med tabs (Dashboard / Devices / Mapping Editor)
- Moderkortsanslutning: läser baseboard-info (Manufacturer/Product/Serial) via WMI på Windows
- Cockpit panel discovery-lager (mock idag, redo att ersättas med HIDSharp)
- Simulator connectors för:
  - MSFS SimConnect
  - X-Plane DataRefs
  - FSUIPC7 via TCP endpoint
- Aircraft-profiler i JSON (`profiles/*.json`)
- Mapping Editor med tabell + Auto-Map + Save Profile
- Snabb test-input i UI för att skicka events till FSUIPC7

## MobiFlight-liknande workflow

UI:t är nu byggt för ett liknande arbetssätt:

1. Se upptäckta enheter och simulatorstatus i **Devices**.
2. Redigera mappings rad-för-rad i **Mapping Editor**.
3. Klicka **Auto-Map** för snabb basmappning (AP1/Heading).
4. Klicka **Save Profile** för att skriva profiler till JSON.
5. Skicka test-input direkt mot FSUIPC7 med top-toolbar.

> Obs: appen efterliknar workflow, men är inte en 1:1-kopia av MobiFlight internt.

## Bygg Windows EXE

Använd scripts som funkar även om du står i `C:\Windows\System32`:

```powershell
C:\path\to\Rowslink\scripts\publish-win-x64.ps1
```

eller:

```bat
C:\path\to\Rowslink\scripts\publish-win-x64.cmd
```

Output:

`src/RowsLink.App/bin/Release/net9.0-windows/win-x64/publish/RowsLink.exe`

## Kör appen

```bat
RowsLink.exe A320
```

(utan argument används `A320`)

## FSUIPC7-anslutning

Sätt endpoint med miljövariabler:

- `ROWSLINK_FSUIPC7_HOST` (default `127.0.0.1`)
- `ROWSLINK_FSUIPC7_PORT` (default `8383`)

Exempel:

```powershell
$env:ROWSLINK_FSUIPC7_HOST="127.0.0.1"
$env:ROWSLINK_FSUIPC7_PORT="8383"
RowsLink.exe A320
```
