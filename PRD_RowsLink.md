# Product Requirements Document (PRD)

## Project Name
**RowsLink**

## Purpose
RowsLink är en desktopapp som kopplar cockpit-hårdvara till flight simulators utan komplex konfiguration. Fokus är **plug-and-play** för Rowsfire-paneler och andra USB-cockpitenheter. Målet är snabb setup, låg latens och ett tydligt UI.

## Problem Statement
Nuvarande lösningar har tydliga brister:

### MobiFlight
- Gammalt gränssnitt
- Lång setup
- Konfigurationer blir stora och röriga
- Ingen auto-mapping

### SPAD.next
- Dyrt
- Svårt för nya användare
- Många funktioner som gör systemet tungt

**Resultat idag:** användare lägger ofta **1–3 timmar** på setup av cockpit-hårdvara.

## Product Goal
En användare kopplar in en panel, startar appen och panelen fungerar direkt inom **5 sekunder**.

## Target Users
- Flight simulator-hobbyister
- Home cockpit-builders
- Användare av Rowsfire-paneler
- MSFS 2020- och MSFS 2024-användare

## Core Features

### 1) Hardware Detection
Appen identifierar automatiskt anslutna cockpit-paneler.

**Krav:**
- Detektera USB HID-enheter
- Identifiera Vendor ID (VID) och Product ID (PID)
- Känna igen Rowsfire-produkter
- Visa ansluten panel i UI

### 2) Simulator Connection
Appen kopplar mot simulatorn.

**Stöd:**
- Microsoft Flight Simulator via **SimConnect**
- X-Plane via **DataRefs**
- FSUIPC-offsets (inklusive **FSUIPC7**)

### 3) Mapping Engine
System som kopplar hårdvaru-inputs till simulator-events.

**Inputs:**
- Push buttons
- Toggle switches
- Rotary encoders
- Potentiometers

**Outputs:**
- LED indicators
- LCD displays
- Backlight control

## Profiles

### Aircraft Profiles
Varje flygplan får en egen profil.

**Funktionalitet:**
- Auto-load av profil när flygplan startar
- Flera profiler per aircraft
- Export/import

## Auto-Mapping
Systemet matchar panelkontroller till aircraft-events.

**Exempel (A320 autopilot-panel):**
- **Knapp:** AP1 → `AUTOPILOT_MASTER`
- **Encoder:** Heading → `HEADING_BUG_INC`, `HEADING_BUG_DEC`

## User Interface

### Dashboard
Visar:
- Anslutna paneler
- Simulatorstatus
- Aktiv aircraft profile

### Device Panel Page
Visar per fysisk panel:
- Knappar
- Encoders
- LED outputs

### Mapping Editor
Användaren klickar på en knapp och väljer event.

## Performance Requirements
- Input latency: **max 20 ms**
- Device polling: **10 ms**
- Startup time: **under 3 sekunder**

## Compatibility

### Operating System
- Windows 10
- Windows 11

### Simulators
- Microsoft Flight Simulator
- Microsoft Flight Simulator 2024
- X-Plane 11
- X-Plane 12

## Technical Architecture

### Backend
- C#
- .NET

### Simulator Integration
- SimConnect SDK

### USB Communication
- HIDSharp

## Plugin System
Plugins lägger till stöd för nya paneler eller aircraft.

**Exempel: Fenix A320-plugin**
- Custom variables
- Special autopilot-events

## Security & Access Levels

### Standardläge
- Mapping
- Profilval

### Adminläge
- Device config
- Plugin-installation

## Future Features
- Cloud Profiles (synka profiler mellan datorer)
- Community Library (delning av aircraft-profiler)
- Auto Updates (automatiska plugin-uppdateringar)

## Success Metrics
- Setup-tid: mål under **5 minuter**
- Stability: crash rate under **0.1%**
- User adoption: **1000 aktiva användare** första året

## Minimum Viable Product (MVP)

### Version 1
- USB device detection
- MSFS SimConnect-anslutning
- Button mapping
- Encoder support
- Aircraft profiles
- Basic UI

### Version 2
- LED outputs
- Auto aircraft detection
- Plugin system
- Cloud profiles

## Vision
RowsLink ska bli standardverktyget för cockpit-hårdvara i flight simulators med snabb setup, stabil anslutning och bättre användarupplevelse än nuvarande alternativ.
