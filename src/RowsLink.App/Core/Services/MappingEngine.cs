using RowsLink.App.Core.Models;

namespace RowsLink.App.Core.Services;

public sealed class MappingEngine
{
    public IEnumerable<SimulatorEvent> Resolve(DeviceInput input, AircraftProfile profile)
    {
        var mapping = profile.Mappings.FirstOrDefault(m =>
            m.DeviceId.Equals(input.DeviceId, StringComparison.OrdinalIgnoreCase)
            && m.ControlId.Equals(input.ControlId, StringComparison.OrdinalIgnoreCase));

        if (mapping is null)
        {
            return Array.Empty<SimulatorEvent>();
        }

        return input.Action switch
        {
            InputAction.Press =>
                [new SimulatorEvent(mapping.OnIncreaseEvent, mapping.Step)],

            InputAction.Increment =>
                [new SimulatorEvent(mapping.OnIncreaseEvent, Math.Max(mapping.Step, input.Delta))],

            InputAction.Decrement when !string.IsNullOrWhiteSpace(mapping.OnDecreaseEvent) =>
                [new SimulatorEvent(mapping.OnDecreaseEvent!, Math.Max(mapping.Step, input.Delta))],

            _ => Array.Empty<SimulatorEvent>()
        };
    }
}
