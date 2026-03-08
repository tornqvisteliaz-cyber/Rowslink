using RowsLink.App.Core.Models;

namespace RowsLink.App.Core.Abstractions;

public interface IHardwareDiscovery
{
    Task<IReadOnlyList<HardwareDevice>> ScanAsync(CancellationToken cancellationToken = default);
}
