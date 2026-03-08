using RowsLink.App.Core.Models;

namespace RowsLink.App.Core.Abstractions;

public interface IProfileStore
{
    Task<IReadOnlyList<AircraftProfile>> GetProfilesAsync(CancellationToken cancellationToken = default);
    Task<AircraftProfile?> FindByAircraftAsync(string aircraft, CancellationToken cancellationToken = default);
    Task SaveAsync(AircraftProfile profile, CancellationToken cancellationToken = default);
}
