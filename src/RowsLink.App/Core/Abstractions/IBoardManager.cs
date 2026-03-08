using RowsLink.App.Core.Models;

namespace RowsLink.App.Core.Abstractions;

public interface IBoardManager
{
    Task<IReadOnlyList<BoardDevice>> ScanBoardsAsync(CancellationToken cancellationToken = default);
    Task<FirmwareFlashResult> FlashFirmwareAsync(FirmwareFlashRequest request, CancellationToken cancellationToken = default);
}
