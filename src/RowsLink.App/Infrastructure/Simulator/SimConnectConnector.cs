using Microsoft.Extensions.Logging;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.Simulator;

public sealed class SimConnectConnector(ILogger<SimConnectConnector> logger)
    : BaseSimulatorConnector(logger)
{
    public override SimulatorKind Kind => SimulatorKind.MsfsSimConnect;
}
