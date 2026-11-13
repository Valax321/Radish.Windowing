using System.Collections;
using System.Numerics;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.Utility;

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

    public void ProcessButtonEvent(in SDL_MouseButtonEvent @event)
    {
        // Do we know about this button?
        var f = (SDL_MouseButtonFlags)(1 << @event.button - 1);
        var btn = f switch
        {
            SDL_MouseButtonFlags.SDL_BUTTON_LMASK => MouseButtons.Mouse1,
            SDL_MouseButtonFlags.SDL_BUTTON_MMASK => MouseButtons.Mouse3,
            SDL_MouseButtonFlags.SDL_BUTTON_RMASK => MouseButtons.Mouse2,
            SDL_MouseButtonFlags.SDL_BUTTON_X1MASK => MouseButtons.Mouse4,
            SDL_MouseButtonFlags.SDL_BUTTON_X2MASK => MouseButtons.Mouse5,
            _ => MouseButtons.None
        };

        // The above check should discard the none value anyway but check it explicitly for robustness.
        if (btn == MouseButtons.None)
            return;

        _buttonStates[ButtonMap[btn]] = @event.down;
        
        if (@event.down)
            ButtonDown?.Invoke(btn, true);
        else
            ButtonUp?.Invoke(btn, false);
    }

    public void ProcessMotionEvent(in SDL_MouseMotionEvent @event)
    {
        Position = new Vector2(@event.x, @event.y);
    }

    public void ProcessWheelEvent(in SDL_MouseWheelEvent @event)
    {
        WheelAxes += new Vector2(@event.x, @event.y);
    }

    public void ClearEvents()
    {
        ButtonDown = null;
        ButtonUp = null;
    }

    public void ClearWheel()
    {
        WheelAxes = Vector2.Zero;
    }

    public void SampleDelta()
    {
        // There's no 'motion stop' or 'wheel stop' events, so we have to set these manually.
        SDL_GetRelativeMouseState(out var dx, out var dy);
        PositionDelta = new Vector2(dx, dy);
    }

    public override string ToString()
    {
        var s = SDL_GetMouseNameForID(InstanceId);
        return string.IsNullOrEmpty(s) ? "Unnamed Mouse" : s;
    }
}