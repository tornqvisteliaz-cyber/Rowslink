using RowsLink.App.Core.Models;

namespace RowsLink.App.Core.Abstractions;

public interface ISimulatorConnector
{
    SimulatorKind Kind { get; }

    Task<SimulatorStatus> ConnectAsync(CancellationToken cancellationToken = default);
    Task<SimulatorStatus> DisconnectAsync(CancellationToken cancellationToken = default);
    Task SendEventAsync(SimulatorEvent simulatorEvent, CancellationToken cancellationToken = default);
}
