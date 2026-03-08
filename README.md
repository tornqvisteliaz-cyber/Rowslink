# RowsLink Windows App (WinForms, EXE-ready)

Detta repo innehåller en riktig **Windows app** (WinForms) i C#/.NET för RowsLink.

## Appfunktioner

- Modern Windows GUI med tabs:
  - Dashboard
  - Devices
  - Mapping Editor
  - Boards & Firmware
- Moderkortsanslutning via WMI (Manufacturer/Product/Serial)
- Board-detektering via serial ports (COM)
- Firmware-uppladdning via `arduino-cli upload`
- Simulator connectors: MSFS SimConnect, X-Plane DataRefs, FSUIPC7 TCP
- Aircraft-profiler i JSON (`profiles/*.json`)
- Mapping Editor med tabell + Auto-Map + Save Profile

## Boards & Firmware

Fliken **Boards & Firmware** gör att du kan:

1. Klicka **Scan Boards** för att hitta anslutna COM-boards.
2. Välja board/port från listan (fyller i Port + Suggested Type).
3. Välja firmware-fil (`.hex`/`.bin`) via **Browse**.
4. Klicka **Upload Firmware** för att köra:

```bash
arduino-cli upload -p <COMx> --fqbn <board_type> --input-file <firmware_file>
```

Exempel på board type:
- `arduino:avr:mega`
- `arduino:avr:nano`
- `arduino:avr:uno`

> Detta matchar tipset i din screenshot: välj korrekt board type om auto-detection inte räcker.

## Bygg Windows EXE

```powershell
C:\path\to\Rowslink\scripts\publish-win-x64.ps1
```

Output:

`src/RowsLink.App/bin/Release/net9.0-windows/win-x64/publish/RowsLink.exe`

## Kör appen

```bat
RowsLink.exe A320
```

## FSUIPC7 endpoint

- `ROWSLINK_FSUIPC7_HOST` (default `127.0.0.1`)
- `ROWSLINK_FSUIPC7_PORT` (default `8383`)
