using Microsoft.Extensions.Logging;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.Simulator;

public sealed class Fsuipc7Connector(ILogger<Fsuipc7Connector> logger)
    : BaseSimulatorConnector(logger)
{
    public override SimulatorKind Kind => SimulatorKind.Fsuipc7;
}
