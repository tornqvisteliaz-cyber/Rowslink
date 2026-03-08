using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;
using RowsLink.App.Core.Services;
using RowsLink.App.Infrastructure.Hardware;
using RowsLink.App.Infrastructure.Boards;
using RowsLink.App.Infrastructure.Profiles;
using RowsLink.App.Infrastructure.Simulator;
using RowsLink.App.Infrastructure.System;
using RowsLink.App.UI;

namespace RowsLink.App;

internal static class Program
{
    [STAThread]
    private static async Task Main(string[] args)
    {
        var aircraft = args.FirstOrDefault() ?? "A320";

        var fsuipcHost = Environment.GetEnvironmentVariable("ROWSLINK_FSUIPC7_HOST") ?? "127.0.0.1";
        var fsuipcPort = int.TryParse(Environment.GetEnvironmentVariable("ROWSLINK_FSUIPC7_PORT"), out var parsedPort)
            ? parsedPort
            : 8383;

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
        services.AddSingleton<IBoardManager, SerialBoardManager>();
        services.AddSingleton<IMotherboardInfoProvider, MotherboardInfoProvider>();
        services.AddSingleton<ISimulatorConnector, SimConnectConnector>();
        services.AddSingleton<ISimulatorConnector, XPlaneConnector>();
        services.AddSingleton<ISimulatorConnector>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Fsuipc7Connector>>();
            return new Fsuipc7Connector(logger, fsuipcHost, fsuipcPort);
        });

        services.AddSingleton<IProfileStore>(_ =>
        {
            var profileDir = ResolveProfileDirectory();
            return new JsonProfileStore(profileDir);
        });

        services.AddSingleton<MappingEngine>();
        services.AddSingleton<RowsLinkRuntime>();
        services.AddSingleton<MainForm>(sp => new MainForm(
            sp.GetRequiredService<RowsLinkRuntime>(),
            sp.GetRequiredService<IProfileStore>(),
            sp.GetRequiredService<IBoardManager>(),
            aircraft,
            fsuipcHost,
            fsuipcPort));

        var provider = services.BuildServiceProvider();
        var profileStore = provider.GetRequiredService<IProfileStore>();
        await SeedFallbackProfileIfEmptyAsync(profileStore);

        ApplicationConfiguration.Initialize();
        Application.Run(provider.GetRequiredService<MainForm>());
    }

    private static string ResolveProfileDirectory()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "profiles"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "profiles")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "profiles"))
        };

        foreach (var candidate in candidates)
        {
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        var defaultDir = Path.Combine(AppContext.BaseDirectory, "profiles");
        Directory.CreateDirectory(defaultDir);
        return defaultDir;
    }

    private static async Task SeedFallbackProfileIfEmptyAsync(IProfileStore profileStore)
    {
        var profiles = await profileStore.GetProfilesAsync();
        if (profiles.Count > 0)
        {
            return;
        }

        var defaultA320 = new AircraftProfile(
            ProfileId: "fallback",
            Aircraft: "A320",
            Name: "Fallback A320 AP",
            Mappings:
            [
                new MappingEntry("rowsfire-a320-ap", "BTN_AP1", "AUTOPILOT_MASTER"),
                new MappingEntry("rowsfire-a320-ap", "ENC_HDG", "HEADING_BUG_INC", "HEADING_BUG_DEC")
            ]);

        await profileStore.SaveAsync(defaultA320);
    }
}
