using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;
using RowsLink.App.Core.Services;
using RowsLink.App.Infrastructure.Hardware;
using RowsLink.App.Infrastructure.Profiles;
using RowsLink.App.Infrastructure.Simulator;
using RowsLink.App.UI;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddSingleton<IHardwareDiscovery, MockHidDeviceDiscovery>();
services.AddSingleton<ISimulatorConnector, SimConnectConnector>();
services.AddSingleton<ISimulatorConnector, XPlaneConnector>();
services.AddSingleton<ISimulatorConnector, Fsuipc7Connector>();
services.AddSingleton<IProfileStore>(_ =>
{
    var profileDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "profiles"));
    return new JsonProfileStore(profileDir);
});
services.AddSingleton<MappingEngine>();
services.AddSingleton<RowsLinkRuntime>();

var provider = services.BuildServiceProvider();
var profileStore = provider.GetRequiredService<IProfileStore>();

await SeedDefaultProfilesAsync(profileStore);

var runtime = provider.GetRequiredService<RowsLinkRuntime>();
var snapshot = await runtime.BootAsync("A320");
ConsoleDashboard.Render(snapshot);

while (true)
{
    DeviceInput input;
    try
    {
        input = ConsoleDashboard.PromptInput();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Invalid input: {ex.Message}");
        continue;
    }

    if (string.IsNullOrWhiteSpace(input.DeviceId))
    {
        break;
    }

    try
    {
        await runtime.RouteInputAsync(input, snapshot.ActiveProfile, SimulatorKind.MsfsSimConnect);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to route input: {ex.Message}");
    }
}

static async Task SeedDefaultProfilesAsync(IProfileStore profileStore)
{
    var profiles = await profileStore.GetProfilesAsync();
    if (profiles.Count > 0)
    {
        return;
    }

    var defaultA320 = new AircraftProfile(
        ProfileId: "default",
        Aircraft: "A320",
        Name: "Default A320 AP",
        Mappings:
        [
            new MappingEntry("rowsfire-a320-ap", "BTN_AP1", "AUTOPILOT_MASTER"),
            new MappingEntry("rowsfire-a320-ap", "ENC_HDG", "HEADING_BUG_INC", "HEADING_BUG_DEC")
        ]);

    await profileStore.SaveAsync(defaultA320);
}
