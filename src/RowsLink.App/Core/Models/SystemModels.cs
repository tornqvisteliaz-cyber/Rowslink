namespace RowsLink.App.Core.Models;

public sealed record MotherboardInfo(
    string Manufacturer,
    string Product,
    string SerialNumber,
    string Source,
    bool IsConnected);
