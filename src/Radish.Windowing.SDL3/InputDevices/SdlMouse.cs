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

    public event MouseButtonStateDelegate? ButtonDown;
    public event MouseButtonStateDelegate? ButtonUp;
    
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
        var f = (SDL.MouseButtonFlags)(1 << @event.Button - 1);
        var btn = f switch
        {
            SDL.MouseButtonFlags.Left => MouseButtons.Mouse1,
            SDL.MouseButtonFlags.Middle => MouseButtons.Mouse3,
            SDL.MouseButtonFlags.Right => MouseButtons.Mouse2,
            SDL.MouseButtonFlags.X1 => MouseButtons.Mouse4,
            SDL.MouseButtonFlags.X2 => MouseButtons.Mouse5,
            _ => MouseButtons.None
        };

        // The above check should discard the none value anyway but check it explicitly for robustness.
        if (btn == MouseButtons.None)
            return;

        _buttonStates[ButtonMap[btn]] = @event.Down;
        
        if (@event.Down)
            ButtonDown?.Invoke(btn, true);
        else
            ButtonUp?.Invoke(btn, false);
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

    public void ClearEvents()
    {
        ButtonDown = null;
        ButtonUp = null;
    }

    public override string ToString()
    {
        var s = SDL.GetMouseNameForID(InstanceId);
        return string.IsNullOrEmpty(s) ? "Unnamed Mouse" : s;
    }
}