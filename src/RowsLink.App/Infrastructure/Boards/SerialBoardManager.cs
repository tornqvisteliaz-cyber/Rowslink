using System.Diagnostics;
using System.IO.Ports;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.Boards;

public sealed class SerialBoardManager : IBoardManager
{
    public Task<IReadOnlyList<BoardDevice>> ScanBoardsAsync(CancellationToken cancellationToken = default)
    {
        var ports = SerialPort.GetPortNames().OrderBy(p => p).ToArray();
        var boards = new List<BoardDevice>();

        foreach (var port in ports)
        {
            boards.Add(new BoardDevice(
                PortName: port,
                DisplayName: $"{port} (Serial Board)",
                SuggestedBoardType: "arduino:avr:mega",
                IsSupportedForAutoFlash: true));
        }

        return Task.FromResult<IReadOnlyList<BoardDevice>>(boards);
    }

    public async Task<FirmwareFlashResult> FlashFirmwareAsync(FirmwareFlashRequest request, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(request.FirmwarePath))
        {
            return new FirmwareFlashResult(false, $"Firmware file not found: {request.FirmwarePath}", "", -1);
        }

        if (string.IsNullOrWhiteSpace(request.PortName) || string.IsNullOrWhiteSpace(request.BoardType))
        {
            return new FirmwareFlashResult(false, "Port and board type are required.", "", -1);
        }

        var args = $"upload -p {request.PortName} --fqbn {request.BoardType} --input-file \"{request.FirmwarePath}\"";
        var cmd = $"arduino-cli {args}";

        var psi = new ProcessStartInfo
        {
            FileName = "arduino-cli",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(psi);
            if (process is null)
            {
                return new FirmwareFlashResult(false, "Failed to start arduino-cli process.", cmd, -1);
            }

            var stdout = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderr = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            var success = process.ExitCode == 0;
            var message = success
                ? $"Firmware uploaded successfully on {request.PortName}.\n{stdout}".Trim()
                : $"Firmware upload failed (exit {process.ExitCode}).\n{stderr}\n{stdout}".Trim();

            return new FirmwareFlashResult(success, message, cmd, process.ExitCode);
        }
        catch (Exception ex)
        {
            return new FirmwareFlashResult(false, $"arduino-cli error: {ex.Message}", cmd, -1);
        }
    }
}
