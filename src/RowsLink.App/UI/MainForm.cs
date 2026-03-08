using System.ComponentModel;
using System.Drawing;
using RowsLink.App.Core.Abstractions;
using RowsLink.App.Core.Models;
using RowsLink.App.Core.Services;

namespace RowsLink.App.UI;

public sealed class MainForm : Form
{
    private readonly RowsLinkRuntime _runtime;
    private readonly IProfileStore _profileStore;
    private readonly string _aircraft;
    private readonly string _fsuipcHost;
    private readonly int _fsuipcPort;

    private RuntimeSnapshot? _snapshot;

    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
    private readonly RichTextBox _dashboardOutput = new() { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.FromArgb(27, 27, 32), ForeColor = Color.Gainsboro, BorderStyle = BorderStyle.None };
    private readonly ListView _devicesList = new() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, GridLines = true };
    private readonly ListView _simStatusList = new() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, GridLines = true };

    private readonly DataGridView _mappingGrid = new() { Dock = DockStyle.Fill, AllowUserToAddRows = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private readonly BindingList<MappingRow> _mappingRows = [];

    private readonly TextBox _deviceId = new() { PlaceholderText = "DeviceId (rowsfire-a320-ap)", Width = 220 };
    private readonly TextBox _controlId = new() { PlaceholderText = "ControlId (BTN_AP1)", Width = 180 };
    private readonly ComboBox _action = new() { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _delta = new() { Width = 80, Minimum = 1, Maximum = 100, Value = 1 };
    private readonly Button _send = new() { Text = "Send", AutoSize = true };
    private readonly Button _refresh = new() { Text = "Refresh", AutoSize = true };
    private readonly Button _autoMap = new() { Text = "Auto-Map", AutoSize = true };
    private readonly Button _saveProfile = new() { Text = "Save Profile", AutoSize = true };

    public MainForm(RowsLinkRuntime runtime, IProfileStore profileStore, string aircraft, string fsuipcHost, int fsuipcPort)
    {
        _runtime = runtime;
        _profileStore = profileStore;
        _aircraft = aircraft;
        _fsuipcHost = fsuipcHost;
        _fsuipcPort = fsuipcPort;

        Text = "RowsLink";
        Width = 1280;
        Height = 820;
        Font = new Font("Segoe UI", 10f);
        BackColor = Color.FromArgb(30, 33, 40);
        ForeColor = Color.Gainsboro;

        InitializeTopToolbar();
        InitializeTabs();

        Controls.Add(_tabs);
        Shown += async (_, _) => await LoadSnapshotAsync();
    }

    private void InitializeTopToolbar()
    {
        _action.DataSource = Enum.GetValues<InputAction>();
        _action.SelectedItem = InputAction.Press;
        _send.Click += OnSendClicked;
        _refresh.Click += async (_, _) => await LoadSnapshotAsync();
        _autoMap.Click += (_, _) => ApplyAutoMapTemplate();
        _saveProfile.Click += async (_, _) => await SaveProfileAsync();

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            AutoSize = false,
            Padding = new Padding(10),
            BackColor = Color.FromArgb(37, 41, 50)
        };

        topPanel.Controls.Add(new Label { Text = "Input:", AutoSize = true, Padding = new Padding(0, 7, 0, 0), ForeColor = Color.Gainsboro });
        topPanel.Controls.AddRange([_deviceId, _controlId, _action, _delta, _send, _refresh, _autoMap, _saveProfile]);
        Controls.Add(topPanel);
    }

    private void InitializeTabs()
    {
        var dashboardPage = new TabPage("Dashboard") { BackColor = Color.FromArgb(30, 33, 40) };
        dashboardPage.Controls.Add(_dashboardOutput);

        var devicesPage = new TabPage("Devices") { BackColor = Color.FromArgb(30, 33, 40) };
        var devicesSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 320 };

        _devicesList.Columns.AddRange([
            new ColumnHeader { Text = "Device", Width = 280 },
            new ColumnHeader { Text = "DeviceId", Width = 220 },
            new ColumnHeader { Text = "VID:PID", Width = 120 },
            new ColumnHeader { Text = "Rowsfire", Width = 100 },
            new ColumnHeader { Text = "Controls", Width = 500 }
        ]);

        _simStatusList.Columns.AddRange([
            new ColumnHeader { Text = "Simulator", Width = 220 },
            new ColumnHeader { Text = "Connected", Width = 120 },
            new ColumnHeader { Text = "Message", Width = 780 }
        ]);

        devicesSplit.Panel1.Controls.Add(_devicesList);
        devicesSplit.Panel2.Controls.Add(_simStatusList);
        devicesPage.Controls.Add(devicesSplit);

        var mappingPage = new TabPage("Mapping Editor") { BackColor = Color.FromArgb(30, 33, 40) };
        ConfigureMappingGrid();
        mappingPage.Controls.Add(_mappingGrid);

        _tabs.TabPages.Add(dashboardPage);
        _tabs.TabPages.Add(devicesPage);
        _tabs.TabPages.Add(mappingPage);
    }

    private void ConfigureMappingGrid()
    {
        _mappingGrid.DataSource = _mappingRows;
        _mappingGrid.RowHeadersVisible = false;
        _mappingGrid.BackgroundColor = Color.FromArgb(22, 24, 30);
        _mappingGrid.DefaultCellStyle.BackColor = Color.FromArgb(37, 41, 50);
        _mappingGrid.DefaultCellStyle.ForeColor = Color.Gainsboro;
        _mappingGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 51, 63);
        _mappingGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        _mappingGrid.EnableHeadersVisualStyles = false;
    }

    private async Task LoadSnapshotAsync()
    {
        try
        {
            _dashboardOutput.Text = "Starting RowsLink..." + Environment.NewLine;
            _snapshot = await _runtime.BootAsync(_aircraft);
            RenderDashboard(_snapshot);
            RenderDevices(_snapshot);
            LoadMappings(_snapshot.ActiveProfile);
        }
        catch (Exception ex)
        {
            _dashboardOutput.Text = "Startup failed: " + ex.Message;
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
            AppendDashboard("DeviceId and ControlId are required.");
            return;
        }

        try
        {
            var input = new DeviceInput(_deviceId.Text.Trim(), _controlId.Text.Trim(), (InputAction)_action.SelectedItem!, (int)_delta.Value);
            await _runtime.RouteInputAsync(input, _snapshot.ActiveProfile, SimulatorKind.Fsuipc7);
            AppendDashboard($"Sent {input.DeviceId}/{input.ControlId} {input.Action} delta={input.Delta} -> {_fsuipcHost}:{_fsuipcPort}");
        }
        catch (Exception ex)
        {
            AppendDashboard("Send failed: " + ex.Message);
        }
    }

    private void ApplyAutoMapTemplate()
    {
        _mappingRows.Clear();
        _mappingRows.Add(new MappingRow { DeviceId = "rowsfire-a320-ap", ControlId = "BTN_AP1", OnIncreaseEvent = "AUTOPILOT_MASTER", OnDecreaseEvent = "", Step = 1 });
        _mappingRows.Add(new MappingRow { DeviceId = "rowsfire-a320-ap", ControlId = "ENC_HDG", OnIncreaseEvent = "HEADING_BUG_INC", OnDecreaseEvent = "HEADING_BUG_DEC", Step = 1 });
        AppendDashboard("Auto-map template applied (MobiFlight-style quick mapping)." );
    }

    private async Task SaveProfileAsync()
    {
        if (_snapshot is null)
        {
            return;
        }

        var mappings = _mappingRows
            .Where(r => !string.IsNullOrWhiteSpace(r.DeviceId) && !string.IsNullOrWhiteSpace(r.ControlId) && !string.IsNullOrWhiteSpace(r.OnIncreaseEvent))
            .Select(r => new MappingEntry(r.DeviceId!.Trim(), r.ControlId!.Trim(), r.OnIncreaseEvent!.Trim(), string.IsNullOrWhiteSpace(r.OnDecreaseEvent) ? null : r.OnDecreaseEvent!.Trim(), r.Step <= 0 ? 1 : r.Step))
            .ToList();

        var updated = _snapshot.ActiveProfile with { Mappings = mappings };
        await _profileStore.SaveAsync(updated);
        _snapshot = _snapshot with { ActiveProfile = updated };
        AppendDashboard($"Profile '{updated.Name}' saved with {mappings.Count} mappings.");
    }

    private void RenderDashboard(RuntimeSnapshot snapshot)
    {
        var lines = new List<string>
        {
            "RowsLink Dashboard",
            "────────────────────────────────────",
            $"Aircraft: {_aircraft}",
            $"Startup: {snapshot.StartupDuration.TotalMilliseconds:N0} ms",
            $"FSUIPC7 endpoint: {_fsuipcHost}:{_fsuipcPort}",
            string.Empty,
            "Motherboard",
            $"  Manufacturer: {snapshot.Motherboard.Manufacturer}",
            $"  Product:      {snapshot.Motherboard.Product}",
            $"  Serial:       {snapshot.Motherboard.SerialNumber}",
            $"  Source:       {snapshot.Motherboard.Source}",
            $"  Connected:    {snapshot.Motherboard.IsConnected}",
            string.Empty,
            $"Connected Panels: {snapshot.Devices.Count}",
            $"Active Profile: {snapshot.ActiveProfile.Name} ({snapshot.ActiveProfile.Aircraft})",
            $"Mappings: {snapshot.ActiveProfile.Mappings.Count}"
        };

        _dashboardOutput.Text = string.Join(Environment.NewLine, lines);
    }

    private void RenderDevices(RuntimeSnapshot snapshot)
    {
        _devicesList.Items.Clear();
        foreach (var device in snapshot.Devices)
        {
            var controls = string.Join(", ", device.Controls.Select(c => $"{c.Label}:{c.Type}"));
            var item = new ListViewItem([device.Name, device.DeviceId, $"{device.VendorId:X4}:{device.ProductId:X4}", device.IsRowsfire.ToString(), controls]);
            _devicesList.Items.Add(item);
        }

        _simStatusList.Items.Clear();
        foreach (var status in snapshot.SimulatorStatuses)
        {
            var item = new ListViewItem([status.Kind.ToString(), status.IsConnected ? "Yes" : "No", status.Message]);
            item.ForeColor = status.IsConnected ? Color.LightGreen : Color.IndianRed;
            _simStatusList.Items.Add(item);
        }
    }

    private void LoadMappings(AircraftProfile profile)
    {
        _mappingRows.Clear();
        foreach (var mapping in profile.Mappings)
        {
            _mappingRows.Add(new MappingRow
            {
                DeviceId = mapping.DeviceId,
                ControlId = mapping.ControlId,
                OnIncreaseEvent = mapping.OnIncreaseEvent,
                OnDecreaseEvent = mapping.OnDecreaseEvent,
                Step = mapping.Step
            });
        }
    }

    private void AppendDashboard(string line)
    {
        _dashboardOutput.AppendText(Environment.NewLine + line);
    }

    public sealed class MappingRow
    {
        public string? DeviceId { get; set; }
        public string? ControlId { get; set; }
        public string? OnIncreaseEvent { get; set; }
        public string? OnDecreaseEvent { get; set; }
        public int Step { get; set; } = 1;
    }
}
