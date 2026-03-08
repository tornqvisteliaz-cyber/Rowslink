using RowsLink.App.Core.Models;
using RowsLink.App.Core.Services;

namespace RowsLink.App.UI;

public sealed class MainForm : Form
{
    private readonly RowsLinkRuntime _runtime;
    private readonly string _aircraft;
    private readonly string _fsuipcHost;
    private readonly int _fsuipcPort;

    private RuntimeSnapshot? _snapshot;

    private readonly TextBox _output = new()
    {
        Multiline = true,
        ScrollBars = ScrollBars.Vertical,
        Dock = DockStyle.Fill,
        ReadOnly = true
    };

    private readonly TextBox _deviceId = new() { PlaceholderText = "DeviceId (rowsfire-a320-ap)", Width = 220 };
    private readonly TextBox _controlId = new() { PlaceholderText = "ControlId (BTN_AP1)", Width = 180 };
    private readonly ComboBox _action = new() { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _delta = new() { Width = 80, Minimum = 1, Maximum = 100, Value = 1 };
    private readonly Button _send = new() { Text = "Send to FSUIPC7", AutoSize = true };
    private readonly Button _refresh = new() { Text = "Refresh", AutoSize = true };

    public MainForm(RowsLinkRuntime runtime, string aircraft, string fsuipcHost, int fsuipcPort)
    {
        _runtime = runtime;
        _aircraft = aircraft;
        _fsuipcHost = fsuipcHost;
        _fsuipcPort = fsuipcPort;

        Text = "RowsLink Windows App";
        Width = 1100;
        Height = 760;

        _action.DataSource = Enum.GetValues<InputAction>();
        _action.SelectedItem = InputAction.Press;
        _send.Click += OnSendClicked;
        _refresh.Click += async (_, _) => await LoadSnapshotAsync();

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            AutoSize = false,
            Padding = new Padding(8)
        };

        topPanel.Controls.AddRange([_deviceId, _controlId, _action, _delta, _send, _refresh]);
        Controls.Add(_output);
        Controls.Add(topPanel);

        Shown += async (_, _) => await LoadSnapshotAsync();
    }

    private async Task LoadSnapshotAsync()
    {
        try
        {
            _output.Text = "Booting RowsLink..." + Environment.NewLine;
            _snapshot = await _runtime.BootAsync(_aircraft);
            RenderSnapshot(_snapshot);
        }
        catch (Exception ex)
        {
            _output.Text = "Startup failed: " + ex.Message;
        }
    }

    private async void OnSendClicked(object? sender, EventArgs e)
    {
        if (_snapshot is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_deviceId.Text) || string.IsNullOrWhiteSpace(_controlId.Text))
        {
            Append("DeviceId and ControlId are required.");
            return;
        }

        try
        {
            var input = new DeviceInput(
                _deviceId.Text.Trim(),
                _controlId.Text.Trim(),
                (InputAction)_action.SelectedItem!,
                (int)_delta.Value);

            await _runtime.RouteInputAsync(input, _snapshot.ActiveProfile, SimulatorKind.Fsuipc7);
            Append($"Sent: {input.DeviceId}/{input.ControlId} {input.Action} delta={input.Delta} -> FSUIPC7 {_fsuipcHost}:{_fsuipcPort}");
        }
        catch (Exception ex)
        {
            Append("Send failed: " + ex.Message);
        }
    }

    private void RenderSnapshot(RuntimeSnapshot snapshot)
    {
        var lines = new List<string>
        {
            "=== RowsLink Windows App ===",
            $"Aircraft: {_aircraft}",
            $"Startup: {snapshot.StartupDuration.TotalMilliseconds:N0} ms",
            $"FSUIPC7 endpoint: {_fsuipcHost}:{_fsuipcPort}",
            string.Empty,
            "Motherboard:",
            $"- Manufacturer: {snapshot.Motherboard.Manufacturer}",
            $"- Product: {snapshot.Motherboard.Product}",
            $"- Serial: {snapshot.Motherboard.SerialNumber}",
            $"- Source: {snapshot.Motherboard.Source}",
            $"- Connected: {snapshot.Motherboard.IsConnected}",
            string.Empty,
            "Connected Panels:"
        };

        foreach (var device in snapshot.Devices)
        {
            lines.Add($"- {device.Name} [{device.DeviceId}] VID:PID {device.VendorId:X4}:{device.ProductId:X4} Rowsfire: {device.IsRowsfire}");
            foreach (var control in device.Controls)
            {
                lines.Add($"  • {control.Label} ({control.Type}) id={control.Id}");
            }
        }

        lines.Add(string.Empty);
        lines.Add("Simulator Status:");
        foreach (var status in snapshot.SimulatorStatuses)
        {
            lines.Add($"- {status.Kind}: {(status.IsConnected ? "Online" : "Offline")} ({status.Message})");
        }

        lines.Add(string.Empty);
        lines.Add("Active Aircraft Profile:");
        lines.Add($"- {snapshot.ActiveProfile.Name} ({snapshot.ActiveProfile.Aircraft})");
        foreach (var mapping in snapshot.ActiveProfile.Mappings)
        {
            lines.Add($"  • {mapping.DeviceId}/{mapping.ControlId} -> {mapping.OnIncreaseEvent} / {mapping.OnDecreaseEvent}");
        }

        _output.Text = string.Join(Environment.NewLine, lines);
    }

    private void Append(string line)
    {
        _output.AppendText(Environment.NewLine + line);
    }
}
