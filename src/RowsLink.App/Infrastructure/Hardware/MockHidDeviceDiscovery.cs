using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;

namespace RowsLink.App.Infrastructure.Hardware;

public sealed class MockHidDeviceDiscovery : IHardwareDiscovery
{
    private static readonly IReadOnlyList<HardwareDevice> Devices =
    [
        new HardwareDevice(
            DeviceId: "rowsfire-a320-ap",
            Name: "Rowsfire A320 Autopilot Panel",
            VendorId: 0x2F01,
            ProductId: 0xA320,
            IsRowsfire: true,
            Controls:
            [
                new ControlDescriptor("BTN_AP1", "AP1", ControlType.PushButton),
                new ControlDescriptor("ENC_HDG", "Heading", ControlType.RotaryEncoder),
                new ControlDescriptor("LED_AP1", "AP1 LED", ControlType.Led)
            ]),

        new HardwareDevice(
            DeviceId: "generic-usb-switch",
            Name: "Generic USB Toggle Panel",
            VendorId: 0x1209,
            ProductId: 0x0001,
            IsRowsfire: false,
            Controls:
            [
                new ControlDescriptor("SW_BAT", "Battery", ControlType.ToggleSwitch)
            ])
    ];

    public Task<IReadOnlyList<HardwareDevice>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Devices);
    }
}
