using RowsLink.App.Core.Models;

namespace RowsLink.App.Core.Abstractions;

public interface IMotherboardInfoProvider
{
    Task<MotherboardInfo> GetAsync(CancellationToken cancellationToken = default);
}
