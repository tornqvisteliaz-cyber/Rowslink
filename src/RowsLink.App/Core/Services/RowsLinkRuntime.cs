using Microsoft.Extensions.Logging;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Core.Services;

public sealed class RowsLinkRuntime(
    IHardwareDiscovery hardwareDiscovery,
    IEnumerable<ISimulatorConnector> simulatorConnectors,
    IProfileStore profileStore,
    MappingEngine mappingEngine,
    ILogger<RowsLinkRuntime> logger)
{
    public async Task<RuntimeSnapshot> BootAsync(string aircraft, CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        var devices = await hardwareDiscovery.ScanAsync(cancellationToken);
        var connectors = simulatorConnectors.ToList();

        var statusList = new List<SimulatorStatus>();
        foreach (var connector in connectors)
        {
            statusList.Add(await connector.ConnectAsync(cancellationToken));
        }

        var profile = await profileStore.FindByAircraftAsync(aircraft, cancellationToken)
            ?? new AircraftProfile("fallback", aircraft, "Fallback profile", []);

        var bootDuration = DateTimeOffset.UtcNow - startedAt;
        logger.LogInformation("RowsLink started with {DeviceCount} devices in {DurationMs} ms", devices.Count, bootDuration.TotalMilliseconds);

        return new RuntimeSnapshot(devices, statusList, profile, bootDuration);
    }

    public async Task RouteInputAsync(
        DeviceInput input,
        AircraftProfile profile,
        SimulatorKind target,
        CancellationToken cancellationToken = default)
    {
        var connector = simulatorConnectors.First(c => c.Kind == target);
        var events = mappingEngine.Resolve(input, profile);

        foreach (var simulatorEvent in events)
        {
            await connector.SendEventAsync(simulatorEvent, cancellationToken);
        }
    }
}

public sealed record RuntimeSnapshot(
    IReadOnlyList<HardwareDevice> Devices,
    IReadOnlyList<SimulatorStatus> SimulatorStatuses,
    AircraftProfile ActiveProfile,
    TimeSpan StartupDuration);
