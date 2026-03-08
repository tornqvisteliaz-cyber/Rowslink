using System.Text.Json;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.Profiles;

public sealed class JsonProfileStore : IProfileStore
{
    private readonly string _directory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public JsonProfileStore(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(_directory);
    }

    public async Task<IReadOnlyList<AircraftProfile>> GetProfilesAsync(CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(_directory, "*.json", SearchOption.TopDirectoryOnly);
        var profiles = new List<AircraftProfile>();

        foreach (var file in files)
        {
            await using var stream = File.OpenRead(file);
            var profile = await JsonSerializer.DeserializeAsync<AircraftProfile>(stream, _jsonOptions, cancellationToken);
            if (profile is not null)
            {
                profiles.Add(profile);
            }
        }

        return profiles;
    }

    public async Task<AircraftProfile?> FindByAircraftAsync(string aircraft, CancellationToken cancellationToken = default)
    {
        var profiles = await GetProfilesAsync(cancellationToken);
        return profiles.FirstOrDefault(p => p.Aircraft.Equals(aircraft, StringComparison.OrdinalIgnoreCase));
    }

    public async Task SaveAsync(AircraftProfile profile, CancellationToken cancellationToken = default)
    {
        var fileName = $"{profile.Aircraft}_{profile.ProfileId}.json".Replace(' ', '_');
        var path = Path.Combine(_directory, fileName);

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, profile, _jsonOptions, cancellationToken);
    }
}
