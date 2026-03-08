# RowsLink Windows App (WinForms, EXE-ready)

Detta repo innehåller en riktig **Windows app** (WinForms) i C#/.NET för RowsLink.

## Appfunktioner

- Windows GUI (inte console-loop)
- Moderkortsanslutning: läser baseboard-info (Manufacturer/Product/Serial) via WMI på Windows
- Cockpit panel discovery-lager (mock idag, redo att ersättas med HIDSharp)
- Simulator connectors för:
  - MSFS SimConnect
  - X-Plane DataRefs
  - FSUIPC7 via TCP endpoint
- Aircraft-profiler i JSON (`profiles/*.json`)
- Input-panel i UI där du skickar knapp/encoder-input till FSUIPC7

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

## Profilbibliotek

- A320 (default, iFly, Fenix)
- A320neo (FBW A32NX)
- B737 (default, PMDG)
- B738 (Zibo)
- B747
- CJ4
- A300
- TBM930
- DC-6
