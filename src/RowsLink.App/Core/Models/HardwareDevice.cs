namespace RowsLink.App.Core.Models;

public sealed record HardwareDevice(
    string DeviceId,
    string Name,
    int VendorId,
    int ProductId,
    bool IsRowsfire,
    IReadOnlyList<ControlDescriptor> Controls);

public sealed record ControlDescriptor(
    string Id,
    string Label,
    ControlType Type);

public enum ControlType
{
    PushButton,
    ToggleSwitch,
    RotaryEncoder,
    Potentiometer,
    Led,
    Lcd,
    Backlight
}
