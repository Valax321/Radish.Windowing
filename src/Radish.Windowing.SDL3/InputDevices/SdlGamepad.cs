using System.Collections;
using System.Numerics;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.Utility;

namespace Radish.Windowing.SDL3.InputDevices;

internal sealed class SdlGamepad : SdlBaseInputDevice, IGamepad
{
    private sealed class SdlTouchpad(ITouchpad.FingerData[] fingers) : ITouchpad
    {
        public IReadOnlyList<ITouchpad.FingerData> Fingers => fingers;

        public ITouchpad.FingerData[] FingersInternal => fingers;
    }
    
    public bool IsConnected => SDL_IsGamepad(InstanceId);
    
    private static readonly Dictionary<GamepadButtons, int>
        ButtonLookup = EnumUtility.GetEnumIndexMap<GamepadButtons>();

    private static readonly Dictionary<GamepadAxes, int> 
        AxisLookup = EnumUtility.GetEnumIndexMap<GamepadAxes>();

    public event GamepadRemappedDelegate? Remapped;
    public event GamepadButtonStateDelegate? ButtonDown;
    public event GamepadButtonStateDelegate? ButtonUp;

    public GamepadModel Model
    {
        get
        {
            var type = SDL_GetGamepadTypeForID(InstanceId);
            return type switch
            {
                SDL_GamepadType.SDL_GAMEPAD_TYPE_XBOX360 => GamepadModel.Xbox360,
                SDL_GamepadType.SDL_GAMEPAD_TYPE_XBOXONE => GamepadModel.XboxOne,
                SDL_GamepadType.SDL_GAMEPAD_TYPE_PS3 => GamepadModel.Dualshock3,
                SDL_GamepadType.SDL_GAMEPAD_TYPE_PS4 => GamepadModel.Dualshock4,
                SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5 => GamepadModel.Dualsense,
                SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_PRO => GamepadModel.SwitchPro,
                SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_LEFT => GamepadModel.SwitchJoyconLeft,
                SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_RIGHT => GamepadModel.SwitchJoyconRight,
                SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_PAIR => GamepadModel.SwitchJoyconPair,
                _ => GamepadModel.Generic
            };
        }
    }

    public int PlayerIndex
    {
        get => SDL_GetGamepadPlayerIndexForID(InstanceId);
        set
        {
            var ptr = SDL_GetGamepadFromID(InstanceId);
            SDL_SetGamepadPlayerIndex(ptr, value < -1 ? -1 : value);
        }
    }

    public IReadOnlyList<ITouchpad> Touchpads => _touchpads;

    private readonly SdlTouchpad[] _touchpads;
    private readonly BitArray _buttonStates = new(ButtonLookup.Count);
    private readonly float[] _axisStates = new float[AxisLookup.Count];
    
    public SdlGamepad(uint instanceId)
    {
        InstanceId = instanceId;

        var ptr = SDL_GetGamepadFromID(InstanceId);
        // What even has more than 1 touchpad? steam deck/new steam controller?
        _touchpads = new SdlTouchpad[SDL_GetNumGamepadTouchpads(ptr)];
        for (var i = 0; i < _touchpads.Length; ++i)
        {
            var fingerCount = SDL_GetNumGamepadTouchpadFingers(ptr, i);
            _touchpads[i] = new SdlTouchpad(new ITouchpad.FingerData[fingerCount]);
            for (var j = 0; j < _touchpads[i].FingersInternal.Length; ++j)
                _touchpads[i].FingersInternal[j] = new ITouchpad.FingerData(new Vector2(float.NaN, float.NaN), -1);
        }
    }

    public void ProcessButtonEvent(in SDL_GamepadButtonEvent @event)
    {
        var rButton = SdlToRadishButtons.GetValueOrDefault((SDL_GamepadButton)@event.button, GamepadButtons.None);
        if (rButton == GamepadButtons.None)
            return;

        _buttonStates[ButtonLookup[rButton]] = @event.down;
        
        if (@event.down)
            ButtonDown?.Invoke(rButton, true);
        else
            ButtonUp?.Invoke(rButton, false);
    }

    public void ProcessAxisEvent(in SDL_GamepadAxisEvent @event)
    {
        var rAxis = SdlToRadishAxes.GetValueOrDefault((SDL_GamepadAxis)@event.axis, GamepadAxes.None);
        if (rAxis == GamepadAxes.None)
            return;
        
        _axisStates[AxisLookup[rAxis]] = (float)@event.value / short.MaxValue;
    }

    public void ProcessTouchpadEvent(in SDL_GamepadTouchpadEvent @event)
    {
        if (@event.touchpad < 0 || @event.touchpad >= _touchpads.Length)
            return;

        var tPad = _touchpads[@event.touchpad];
        if (@event.finger < 0 || @event.finger >= tPad.FingersInternal.Length)
            return;
        
        if (@event.type == SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_UP)
        {
            tPad.FingersInternal[@event.finger] = new ITouchpad.FingerData(new Vector2(float.NaN, float.NaN), -1);
        }
        else
        {
            tPad.FingersInternal[@event.finger] =
                new ITouchpad.FingerData(new Vector2(@event.x, @event.y), @event.pressure);
        }
    }
    
    public bool IsPressed(GamepadButtons button)
    {
        if (button == GamepadButtons.None)
            return false;

        return _buttonStates[ButtonLookup[button]];
    }

    public float GetAxisValue(GamepadAxes axis)
    {
        if (axis == GamepadAxes.None)
            return 0.0f;

        return _axisStates[AxisLookup[axis]];
    }

    public void ProcessRemapEvent() => Remapped?.Invoke();

    private static readonly Dictionary<SDL_GamepadButton, GamepadButtons> SdlToRadishButtons = new()
    {
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH, GamepadButtons.FacePadNorth },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH, GamepadButtons.FacePadSouth },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST, GamepadButtons.FacePadEast },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST, GamepadButtons.FacePadWest },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP, GamepadButtons.DPadNorth },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN, GamepadButtons.DPadSouth },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT, GamepadButtons.DPadWest },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT, GamepadButtons.DPadEast },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER, GamepadButtons.LeftShoulder },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER, GamepadButtons.RightShoulder },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START, GamepadButtons.Start },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK, GamepadButtons.Select },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK, GamepadButtons.LeftStickPress },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK, GamepadButtons.RightStickPress },
        { SDL_GamepadButton.SDL_GAMEPAD_BUTTON_TOUCHPAD, GamepadButtons.TouchpadPress },
    };

    private static readonly Dictionary<SDL_GamepadAxis, GamepadAxes> SdlToRadishAxes = new()
    {
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER, GamepadAxes.LeftTrigger },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER, GamepadAxes.RightTrigger },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX, GamepadAxes.LeftStickX },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY, GamepadAxes.LeftStickY },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX, GamepadAxes.RightStickX },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY, GamepadAxes.RightStickY }
    };

    public void ClearEvents()
    {
        Remapped = null;
        ButtonDown = null;
        ButtonUp = null;
    }

    public override string ToString()
    {
        var s = SDL_GetGamepadNameForID(InstanceId);
        return string.IsNullOrEmpty(s) ? "Unnamed Gamepad" : s;
    }
}