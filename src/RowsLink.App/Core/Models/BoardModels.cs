namespace RowsLink.App.Core.Models;

public sealed record BoardDevice(
    string PortName,
    string DisplayName,
    string SuggestedBoardType,
    bool IsSupportedForAutoFlash);

public sealed record FirmwareFlashRequest(
    string PortName,
    string BoardType,
    string FirmwarePath);

public sealed record FirmwareFlashResult(
    bool Success,
    string Message,
    string Command,
    int ExitCode);
