using Microsoft.Extensions.Logging;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.Simulator;

public sealed class XPlaneConnector(ILogger<XPlaneConnector> logger)
    : BaseSimulatorConnector(logger)
{
    public override SimulatorKind Kind => SimulatorKind.XPlaneDataRef;
}
