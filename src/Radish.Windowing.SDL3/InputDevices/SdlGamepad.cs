using System.Collections;
using System.Numerics;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.Utility;
using SDL3;

namespace Radish.Windowing.SDL3.InputDevices;

internal sealed class SdlGamepad : SdlBaseInputDevice, IGamepad
{
    private sealed class SdlTouchpad(ITouchpad.FingerData[] fingers) : ITouchpad
    {
        public IReadOnlyList<ITouchpad.FingerData> Fingers => fingers;

        public ITouchpad.FingerData[] FingersInternal => fingers;
    }
    
    private static readonly Dictionary<GamepadButtons, int>
        ButtonLookup = EnumUtility.GetEnumIndexMap<GamepadButtons>();

    private static readonly Dictionary<GamepadAxes, int> 
        AxisLookup = EnumUtility.GetEnumIndexMap<GamepadAxes>();

    public event GamepadRemappedDelegate? Remapped;

    public GamepadModel Model
    {
        get
        {
            var type = SDL.GetGamepadTypeForID(InstanceId);
            return type switch
            {
                SDL.GamepadType.Xbox360 => GamepadModel.Xbox360,
                SDL.GamepadType.XboxOne => GamepadModel.XboxOne,
                SDL.GamepadType.PS3 => GamepadModel.Dualshock3,
                SDL.GamepadType.PS4 => GamepadModel.Dualshock4,
                SDL.GamepadType.PS5 => GamepadModel.Dualsense,
                SDL.GamepadType.NintendoSwitchPro => GamepadModel.SwitchPro,
                SDL.GamepadType.NintendoSwitchJoyconLeft => GamepadModel.SwitchJoyconLeft,
                SDL.GamepadType.NintendoSwitchJoyconRight => GamepadModel.SwitchJoyconRight,
                SDL.GamepadType.NintendoSwitchJoyconPair => GamepadModel.SwitchJoyconPair,
                _ => GamepadModel.Generic
            };
        }
    }

    public IReadOnlyList<ITouchpad> Touchpads => _touchpads;

    private readonly SdlTouchpad[] _touchpads;
    private readonly BitArray _buttonStates = new(ButtonLookup.Count);
    private readonly float[] _axisStates = new float[AxisLookup.Count];
    
    public SdlGamepad(uint instanceId)
    {
        InstanceId = instanceId;

        var ptr = SDL.GetGamepadFromID(InstanceId);
        // What even has more than 1 touchpad? steam deck/new steam controller?
        _touchpads = new SdlTouchpad[SDL.GetNumGamepadTouchpads(ptr)];
        for (var i = 0; i < _touchpads.Length; ++i)
        {
            var fingerCount = SDL.GetNumGamepadTouchpadFingers(ptr, i);
            _touchpads[i] = new SdlTouchpad(new ITouchpad.FingerData[fingerCount]);
            for (var j = 0; j < _touchpads[i].FingersInternal.Length; ++j)
                _touchpads[i].FingersInternal[j] = new ITouchpad.FingerData(new Vector2(float.NaN, float.NaN), -1);
        }
    }

    public void ProcessButtonEvent(in SDL.GamepadButtonEvent @event)
    {
        var rButton = SdlToRadishButtons.GetValueOrDefault((SDL.GamepadButton)@event.Button, GamepadButtons.None);
        if (rButton == GamepadButtons.None)
            return;

        _buttonStates[ButtonLookup[rButton]] = @event.Down;
    }

    public void ProcessAxisEvent(in SDL.GamepadAxisEvent @event)
    {
        var rAxis = SdlToRadishAxes.GetValueOrDefault((SDL.GamepadAxis)@event.Axis, GamepadAxes.None);
        if (rAxis == GamepadAxes.None)
            return;
        
        _axisStates[AxisLookup[rAxis]] = (float)@event.Value / short.MaxValue;
    }

    public void ProcessTouchpadEvent(in SDL.GamepadTouchpadEvent @event)
    {
        if (@event.Touchpad < 0 || @event.Touchpad >= _touchpads.Length)
            return;

        var tPad = _touchpads[@event.Touchpad];
        if (@event.Finger < 0 || @event.Finger >= tPad.FingersInternal.Length)
            return;
        
        if (@event.Type == SDL.EventType.GamepadTouchpadUp)
        {
            tPad.FingersInternal[@event.Finger] = new ITouchpad.FingerData(new Vector2(float.NaN, float.NaN), -1);
        }
        else
        {
            tPad.FingersInternal[@event.Finger] =
                new ITouchpad.FingerData(new Vector2(@event.X, @event.Y), @event.Pressure);
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

    private static readonly Dictionary<SDL.GamepadButton, GamepadButtons> SdlToRadishButtons = new()
    {
        { SDL.GamepadButton.North, GamepadButtons.FacePadNorth },
        { SDL.GamepadButton.South, GamepadButtons.FacePadSouth },
        { SDL.GamepadButton.East, GamepadButtons.FacePadEast },
        { SDL.GamepadButton.West, GamepadButtons.FacePadWest },
        { SDL.GamepadButton.DPadUp, GamepadButtons.DPadNorth },
        { SDL.GamepadButton.DPadDown, GamepadButtons.DPadSouth },
        { SDL.GamepadButton.DPadLeft, GamepadButtons.DPadWest },
        { SDL.GamepadButton.DPadRight, GamepadButtons.DPadEast },
        { SDL.GamepadButton.LeftShoulder, GamepadButtons.LeftShoulder },
        { SDL.GamepadButton.RightShoulder, GamepadButtons.RightShoulder },
        { SDL.GamepadButton.Start, GamepadButtons.Start },
        { SDL.GamepadButton.Back, GamepadButtons.Select },
        { SDL.GamepadButton.LeftStick, GamepadButtons.LeftStickPress },
        { SDL.GamepadButton.RightStick, GamepadButtons.RightStickPress },
        { SDL.GamepadButton.Touchpad, GamepadButtons.TouchpadPress },
    };

    private static readonly Dictionary<SDL.GamepadAxis, GamepadAxes> SdlToRadishAxes = new()
    {
        { SDL.GamepadAxis.LeftTrigger, GamepadAxes.LeftTrigger },
        { SDL.GamepadAxis.RightTrigger, GamepadAxes.RightTrigger },
        { SDL.GamepadAxis.LeftX, GamepadAxes.LeftStickX },
        { SDL.GamepadAxis.LeftY, GamepadAxes.LeftStickY },
        { SDL.GamepadAxis.RightX, GamepadAxes.RightStickX },
        { SDL.GamepadAxis.RightY, GamepadAxes.RightStickY }
    };
}