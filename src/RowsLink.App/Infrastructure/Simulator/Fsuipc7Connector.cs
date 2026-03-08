using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.Simulator;

public sealed class Fsuipc7Connector(
    ILogger<Fsuipc7Connector> logger,
    string host,
    int port) : ISimulatorConnector
{
    private TcpClient? _client;

    public SimulatorKind Kind => SimulatorKind.Fsuipc7;

    public async Task<SimulatorStatus> ConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port, cancellationToken);
            return new SimulatorStatus(Kind, true, $"Connected to FSUIPC7 endpoint {host}:{port}", DateTimeOffset.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to FSUIPC7 endpoint {Host}:{Port}", host, port);
            _client?.Dispose();
            _client = null;
            return new SimulatorStatus(Kind, false, $"Failed: {ex.Message}", DateTimeOffset.UtcNow);
        }
    }

    public Task<SimulatorStatus> DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _client?.Dispose();
        _client = null;
        return Task.FromResult(new SimulatorStatus(Kind, false, "Disconnected", DateTimeOffset.UtcNow));
    }

    public async Task SendEventAsync(SimulatorEvent simulatorEvent, CancellationToken cancellationToken = default)
    {
        if (_client is null || !_client.Connected)
        {
            throw new InvalidOperationException("FSUIPC7 is not connected. Start FSUIPC7 endpoint first.");
        }

        var payload = $"EVENT {simulatorEvent.EventName} {simulatorEvent.Value}\n";
        var bytes = Encoding.UTF8.GetBytes(payload);
        await _client.GetStream().WriteAsync(bytes, cancellationToken);
    }
}
