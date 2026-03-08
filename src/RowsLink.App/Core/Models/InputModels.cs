namespace RowsLink.App.Core.Models;

public sealed record DeviceInput(
    string DeviceId,
    string ControlId,
    InputAction Action,
    int Delta = 1);

public enum InputAction
{
    Press,
    Release,
    Increment,
    Decrement,
    SetValue
}
