using System.Collections;
using System.Numerics;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.Utility;
using SDL3;

namespace Radish.Windowing.SDL3.InputDevices;

internal sealed class SdlMouse : SdlBaseInputDevice, IMouse
{
    private static readonly Dictionary<MouseButtons, int> ButtonMap 
        = EnumUtility.GetEnumIndexMap<MouseButtons>();
    
    public Vector2 Position { get; private set; }
    public Vector2 PositionDelta { get; private set; }
    public Vector2 WheelAxes { get; private set; }

    private readonly BitArray _buttonStates;
    
    public SdlMouse(uint instanceId)
    {
        InstanceId = instanceId;

        _buttonStates = new BitArray(ButtonMap.Count);
    }
    
    public bool IsPressed(MouseButtons button)
    {
        if (button == MouseButtons.None)
            return false;

        return _buttonStates[ButtonMap[button]];
    }

    public void ProcessButtonEvent(in SDL.MouseButtonEvent @event)
    {
        // Do we know about this button?
        var btn = (MouseButtons)(@event.Button + 1); // mouse button indices start at 0, mouse1 starts at 1
        if (!Enum.IsDefined(btn))
            return;

        // The above check should discard the none value anyway but check it explicitly for robustness.
        if (btn == MouseButtons.None)
            return;

        _buttonStates[ButtonMap[btn]] = @event.Down;
    }

    public void ProcessMotionEvent(in SDL.MouseMotionEvent @event)
    {
        Position = new Vector2(@event.X, @event.Y);
        PositionDelta = new Vector2(@event.XRel, @event.YRel);
    }

    public void ProcessWheelEvent(in SDL.MouseWheelEvent @event)
    {
        WheelAxes = new Vector2(@event.X, @event.Y);
    }
}