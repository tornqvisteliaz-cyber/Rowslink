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
