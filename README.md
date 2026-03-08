# RowsLink MVP App (EXE-ready)

Detta repo innehåller en körbar **RowsLink-app** i C#/.NET med stöd för profiler, mapping och simulator-connector-arkitektur.

## Implementerat

- USB panel discovery-lager (mock idag, redo att ersättas med HIDSharp)
- Simulator connectors för:
  - MSFS SimConnect
  - X-Plane DataRefs
  - FSUIPC7
- Mapping engine för knapp/encoder-input
- Profilsystem med många färdiga aircraft-profiler (`profiles/*.json`)
- Dashboard i konsol (MVP-UI)
- Runtime som bootar enheter + simulatorstatus + aktiv profil

## Viktig fix för `MSB1009: Project file does not exist`

Felet uppstår när man kör `dotnet publish src/RowsLink.App/RowsLink.App.csproj ...` från fel working directory (t.ex. `C:\Windows\System32`).

Använd i stället scriptet i `scripts/` som hittar repo-root automatiskt.

### Alternativ 1 (PowerShell)

```powershell
# fungerar även om du står i C:\Windows\System32
C:\path\to\Rowslink\scripts\publish-win-x64.ps1
```

### Alternativ 2 (CMD)

```bat
C:\path\to\Rowslink\scripts\publish-win-x64.cmd
```

## Manuell build av Windows EXE

Kör från **repo root**:

```bash
dotnet publish src/RowsLink.App/RowsLink.App.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Output-exe blir:

`src/RowsLink.App/bin/Release/net9.0-windows/win-x64/publish/RowsLink.exe`

## Kör appen

```bash
RowsLink.exe A320
```

(utan argument används `A320`)

## Profiler

Appen laddar profiler från `profiles/` och väljer aktiv profil via aircraft-namn.

Exempel:
- A320 (default, iFly, Fenix)
- A320neo (FBW A32NX)
- B737 (default, PMDG)
- B738 (Zibo)
- B747
- CJ4
- A300
- TBM930
- DC-6

## Nästa steg

1. Implementera riktig HIDSharp-device detection.
2. Koppla in riktig SimConnect-/X-Plane-/FSUIPC7-I/O.
3. Lägg till desktop UI (WPF/WinUI/Avalonia).
4. Lägg till plugin-loader och auto-aircraft-detection från simulator.
