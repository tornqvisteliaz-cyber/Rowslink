 codex/outline-product-requirements-for-rowslink-gleech
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

## Bygg Windows EXE

Kör på en maskin med .NET 8 SDK installerat:

```bash
dotnet publish src/RowsLink.App/RowsLink.App.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Output-exe blir:

`src/RowsLink.App/bin/Release/net8.0-windows/win-x64/publish/RowsLink.exe`

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

# RowsLink MVP (kodskelett)

Detta repo innehåller en första körbar MVP-arkitektur för **RowsLink** baserat på kraven i `PRD_RowsLink.md`.

## Vad som finns implementerat

- **Hardware detection-lager** via `IHardwareDiscovery` + mockad HID-device scan.
- **Simulator connectors** för:
  - MSFS SimConnect
  - X-Plane DataRefs
  - FSUIPC7
- **Mapping engine** som mappar device-input till simulator events.
- **Aircraft profiles** i JSON med auto-seeding av A320-profil.
- **Dashboard (console-UI)** som visar:
  - anslutna paneler
  - simulatorstatus
  - aktiv profil
- **Input-routing loop** för att simulera knapp/encoder-input.

## Projektstruktur

- `src/RowsLink.App/Core` – domänmodeller, interfaces, tjänster
- `src/RowsLink.App/Infrastructure` – mockad hårdvara, simulatorkopplingar, JSON profil-store
- `src/RowsLink.App/UI` – enkel dashboard i konsol
- `src/RowsLink.App/Program.cs` – composition root och runtime

## Nästa steg mot riktig produktion

1. Byt `MockHidDeviceDiscovery` mot riktig HIDSharp-implementation.
2. Implementera verklig SimConnect-/X-Plane-/FSUIPC7-I/O.
3. Ersätt console-UI med desktop-UI (WPF/WinUI/Avalonia).
4. Lägg till latency-mätning och 10 ms polling-loop.
5. Lägg till export/import samt auto-load per aircraft från simulator-state.
 main
