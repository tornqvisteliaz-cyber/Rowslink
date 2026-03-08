namespace RowsLink.App.Core.Models;

public enum SimulatorKind
{
    MsfsSimConnect,
    XPlaneDataRef,
    Fsuipc7
}

public sealed record SimulatorStatus(
    SimulatorKind Kind,
    bool IsConnected,
    string Message,
    DateTimeOffset Timestamp);

public sealed record SimulatorEvent(
    string EventName,
    int Value = 0);
