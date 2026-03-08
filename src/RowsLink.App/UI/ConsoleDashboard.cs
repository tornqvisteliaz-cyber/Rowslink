using RowsLink.App.Core.Models;
using RowsLink.App.Core.Services;

namespace RowsLink.App.UI;

public static class ConsoleDashboard
{
    public static void Render(RuntimeSnapshot snapshot)
    {
        Console.WriteLine("=== RowsLink Dashboard ===");
        Console.WriteLine($"Startup: {snapshot.StartupDuration.TotalMilliseconds:N0} ms");
        Console.WriteLine();

        Console.WriteLine("Connected Panels:");
        foreach (var device in snapshot.Devices)
        {
            Console.WriteLine($"- {device.Name} [{device.DeviceId}] VID:PID {device.VendorId:X4}:{device.ProductId:X4} Rowsfire: {device.IsRowsfire}");
            foreach (var control in device.Controls)
            {
                Console.WriteLine($"  • {control.Label} ({control.Type}) id={control.Id}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Simulator Status:");
        foreach (var status in snapshot.SimulatorStatuses)
        {
            Console.WriteLine($"- {status.Kind}: {(status.IsConnected ? "Online" : "Offline")} ({status.Message})");
        }

        Console.WriteLine();
        Console.WriteLine("Active Aircraft Profile:");
        Console.WriteLine($"- {snapshot.ActiveProfile.Name} ({snapshot.ActiveProfile.Aircraft})");
        foreach (var mapping in snapshot.ActiveProfile.Mappings)
        {
            Console.WriteLine($"  • {mapping.DeviceId}/{mapping.ControlId} -> {mapping.OnIncreaseEvent} / {mapping.OnDecreaseEvent}");
        }
    }

    public static DeviceInput PromptInput()
    {
        Console.WriteLine();
        Console.WriteLine("Simulate input (format: deviceId controlId action [delta]) or 'exit':");
        var line = Console.ReadLine();

        if (string.Equals(line, "exit", StringComparison.OrdinalIgnoreCase))
        {
            return new DeviceInput("", "", InputAction.Release, 0);
        }

        var parts = (line ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
        {
            throw new InvalidOperationException("Expected: deviceId controlId action [delta]");
        }

        var action = Enum.Parse<InputAction>(parts[2], ignoreCase: true);
        var delta = parts.Length >= 4 ? int.Parse(parts[3]) : 1;

        return new DeviceInput(parts[0], parts[1], action, delta);
    }
}
