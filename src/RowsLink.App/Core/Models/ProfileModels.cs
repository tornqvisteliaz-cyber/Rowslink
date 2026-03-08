namespace RowsLink.App.Core.Models;

public sealed record AircraftProfile(
    string ProfileId,
    string Aircraft,
    string Name,
    List<MappingEntry> Mappings);

public sealed record MappingEntry(
    string DeviceId,
    string ControlId,
    string OnIncreaseEvent,
    string? OnDecreaseEvent = null,
    int Step = 1);
