using Microsoft.Extensions.Logging;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.Simulator;

public abstract class BaseSimulatorConnector(ILogger logger) : ISimulatorConnector
{
    public abstract SimulatorKind Kind { get; }

    private bool _connected;

    public virtual Task<SimulatorStatus> ConnectAsync(CancellationToken cancellationToken = default)
    {
        _connected = true;
        return Task.FromResult(new SimulatorStatus(Kind, true, "Connected (mock)", DateTimeOffset.UtcNow));
    }

    public virtual Task<SimulatorStatus> DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _connected = false;
        return Task.FromResult(new SimulatorStatus(Kind, false, "Disconnected", DateTimeOffset.UtcNow));
    }

    public virtual Task SendEventAsync(SimulatorEvent simulatorEvent, CancellationToken cancellationToken = default)
    {
        if (!_connected)
        {
            throw new InvalidOperationException($"{Kind} is not connected.");
        }

        logger.LogInformation("[{Simulator}] Event: {EventName} Value: {Value}", Kind, simulatorEvent.EventName, simulatorEvent.Value);
        return Task.CompletedTask;
    }
}
