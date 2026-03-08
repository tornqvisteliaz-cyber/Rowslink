using System.Diagnostics;
using System.Runtime.InteropServices;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.System;

public sealed class MotherboardInfoProvider : IMotherboardInfoProvider
{
    public async Task<MotherboardInfo> GetAsync(CancellationToken cancellationToken = default)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new MotherboardInfo("Unknown", "Unknown", "Unknown", "Non-Windows host", false);
        }

        var psi = new ProcessStartInfo
        {
            FileName = "wmic",
            Arguments = "baseboard get Manufacturer,Product,SerialNumber /format:csv",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process is null)
        {
            return new MotherboardInfo("Unknown", "Unknown", "Unknown", "wmic start failed", false);
        }

        var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        var dataLine = output
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(l => l.Contains(",", StringComparison.Ordinal) && !l.Contains("Manufacturer", StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(dataLine))
        {
            return new MotherboardInfo("Unknown", "Unknown", "Unknown", "wmic returned no data", false);
        }

        var parts = dataLine.Split(',');
        if (parts.Length < 4)
        {
            return new MotherboardInfo("Unknown", "Unknown", "Unknown", "wmic parse failed", false);
        }

        return new MotherboardInfo(
            Manufacturer: parts[1].Trim(),
            Product: parts[2].Trim(),
            SerialNumber: parts[3].Trim(),
            Source: "WMI baseboard",
            IsConnected: true);
    }
}
