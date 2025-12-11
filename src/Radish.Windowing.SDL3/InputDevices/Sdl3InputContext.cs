using System.Drawing;
using System.Runtime.InteropServices.Marshalling;
using JetBrains.Annotations;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.Utility;
using SDL3;

namespace Radish.Windowing.SDL3.InputDevices;

/// <summary>
/// Input context backed by an SDL3 window.
/// </summary>
[PublicAPI]
public class Sdl3InputContext : IInputContext
{
    #region Public Properties
    
    /// <inheritdoc/>
    public event TextInputDelegate? TextInput;

    /// <inheritdoc/>
    public event InputDeviceChangeDelegate? DeviceAdded;
    
    /// <inheritdoc/>
    public event InputDeviceChangeDelegate? DeviceRemoved;

    /// <inheritdoc/>
    public IReadOnlyCollection<IInputDevice> InputDevices => _devices;

    /// <inheritdoc/>
    public IReadOnlyCollection<IKeyboard> Keyboards => _keyboards.Values;

    /// <inheritdoc/>
    public IReadOnlyCollection<IMouse> Mice => _mice.Values;

    /// <inheritdoc/>
    public IReadOnlyCollection<IGamepad> Gamepads => _gamepads.Values;
    
    #endregion

    #region Private Fields
    
    private readonly Sdl3Window _owner;
    
    private readonly List<SdlBaseInputDevice> _devices = [];
    private readonly List<SdlBaseInputDevice> _removedDevices = [];
    
    private readonly Dictionary<uint, SdlGamepad> _gamepads = [];
    private readonly Dictionary<uint, SdlKeyboard> _keyboards = [];
    private readonly Dictionary<uint, SdlMouse> _mice = [];
    
    #endregion

    #region Constructor
    
    internal Sdl3InputContext(Sdl3Window owner)
    {
        _owner = owner;
        _owner.EventProcess += OnEvent;
    }
    
    #endregion
    
    #region SDL Event Handling
    
    private unsafe void OnEvent(in SDL.Event @event)
    {
        switch ((SDL.EventType)@event.Type)
        {
            // Input device add/removed
            case SDL.EventType.GamepadAdded:
            case SDL.EventType.GamepadRemoved:
                HandleGamepadConnectionEvent(in @event);
                break;
            case SDL.EventType.KeyboardAdded:
            case SDL.EventType.KeyboardRemoved:
                HandleKeyboardConnectionEvent(in @event);
                break;
            case SDL.EventType.MouseAdded:
            case SDL.EventType.MouseRemoved:
                HandleMouseConnectionEvent(in @event);
                break;
            
            // Text input
            case SDL.EventType.TextInput:
                var t = Utf8StringMarshaller.ConvertToManaged((byte*)@event.Text.Text);
                TextInput?.Invoke(t ?? ReadOnlySpan<char>.Empty);
                break;
            
            // keyboard input
            case SDL.EventType.KeyDown:
            case SDL.EventType.KeyUp:
                HandleKeyboardKeyEvent(in @event);
                break;
            
            // gamepad input
            case SDL.EventType.GamepadButtonDown:
            case SDL.EventType.GamepadButtonUp:
                HandleGamepadButtonEvent(in @event);
                break;
            case SDL.EventType.GamepadAxisMotion:
                HandleGamepadAxisEvent(in @event);
                break;
            case SDL.EventType.GamepadTouchpadDown:
            case SDL.EventType.GamepadTouchpadUp:
            case SDL.EventType.GamepadTouchpadMotion:
                HandleGamepadTouchEvent(in @event);
                break;
            
            // mouse input
            case SDL.EventType.MouseButtonDown:
            case SDL.EventType.MouseButtonUp:
                HandleMouseButtonEvent(in @event);
                break;
            case SDL.EventType.MouseMotion:
                HandleMouseMotionEvent(in @event);
                break;
            case SDL.EventType.MouseWheel:
                break;
            
            case SDL.EventType.GamepadRemapped:
                HandleRemappedEvent(in @event);
                break;
        }
    }

    private void HandleKeyboardKeyEvent(in SDL.Event @event)
    {
        var kbId = @event.Key.Which;
        var kb = _keyboards.GetValueOrDefault(kbId);
        kb?.ProcessKeyEvent(in @event.Key);
    }
    
    private void HandleKeyboardConnectionEvent(in SDL.Event @event)
    {
        var gpId = @event.KDevice.Which;

        if (@event.GDevice.Type == SDL.EventType.KeyboardAdded)
        {
            // I'm excusing this use of linq because device add functions are pretty rare
            var existingGamepad = _removedDevices
                                      .OfType<SdlKeyboard>()
                                      .FirstOrDefault(static (x, gpId) => x.InstanceId == gpId, gpId) 
                                  ?? new SdlKeyboard(gpId);
        
            AddInputDevice(existingGamepad);
        }
        else if (@event.GDevice.Type == SDL.EventType.KeyboardRemoved)
        {
            if (!_keyboards.TryGetValue(gpId, out var existingGamepad))
            {
                // If this ever happens it means we didn't catch a gamepad being inserted, which is a failure on our part.
                throw new InvalidOperationException(
                    "No existing keyboard instance in lookup table, this is a bug in Radish.Windowing (or less likely, SDL3)");
            }
        
            RemoveInputDevice(existingGamepad);
        }
    }
    
    private void HandleGamepadConnectionEvent(in SDL.Event @event)
    {
        var gpId = @event.GDevice.Which;

        if (@event.GDevice.Type == SDL.EventType.GamepadAdded)
        {
            // I'm excusing this use of linq because device add functions are pretty rare
            var existingGamepad = _removedDevices
                                      .OfType<SdlGamepad>()
                                      .FirstOrDefault(static (x, gpId) => x.InstanceId == gpId, gpId) 
                                  ?? new SdlGamepad(gpId);
        
            AddInputDevice(existingGamepad);
        }
        else if (@event.GDevice.Type == SDL.EventType.GamepadRemoved)
        {
            if (!_gamepads.TryGetValue(gpId, out var existingGamepad))
            {
                // If this ever happens it means we didn't catch a gamepad being inserted, which is a failure on our part.
                throw new InvalidOperationException(
                    "No existing gamepad instance in lookup table, this is a bug in Radish.Windowing (or less likely, SDL3)");
            }
        
            RemoveInputDevice(existingGamepad);
        }
    }
    
    private void HandleGamepadButtonEvent(in SDL.Event @event)
    {
        var gpId = @event.GButton.Which;
        var gp = _gamepads.GetValueOrDefault(gpId);
        gp?.ProcessButtonEvent(in @event.GButton);
    }
    
    private void HandleGamepadAxisEvent(in SDL.Event @event)
    {
        var gpId = @event.GAxis.Which;
        var gp = _gamepads.GetValueOrDefault(gpId);
        gp?.ProcessAxisEvent(in @event.GAxis);
    }
    
    private void HandleGamepadTouchEvent(in SDL.Event @event)
    {
        var gpId = @event.GTouchpad.Which;
        var gp = _gamepads.GetValueOrDefault(gpId);
        gp?.ProcessTouchpadEvent(in @event.GTouchpad);
    }
    
    private void HandleRemappedEvent(in SDL.Event @event)
    {
        var gpId = @event.GDevice.Which;
        var gp = _gamepads.GetValueOrDefault(gpId);
        gp?.ProcessRemapEvent();
    }
    
    private void HandleMouseConnectionEvent(in SDL.Event @event)
    {
        var gpId = @event.MDevice.Which;

        if (@event.GDevice.Type == SDL.EventType.MouseAdded)
        {
            // I'm excusing this use of linq because device add functions are pretty rare
            var existingGamepad = _removedDevices
                                      .OfType<SdlMouse>()
                                      .FirstOrDefault(static (x, gpId) => x.InstanceId == gpId, gpId) 
                                  ?? new SdlMouse(gpId);
        
            AddInputDevice(existingGamepad);
        }
        else if (@event.GDevice.Type == SDL.EventType.MouseRemoved)
        {
            if (!_mice.TryGetValue(gpId, out var existingGamepad))
            {
                // If this ever happens it means we didn't catch a gamepad being inserted, which is a failure on our part.
                throw new InvalidOperationException(
                    "No existing keyboard instance in lookup table, this is a bug in Radish.Windowing (or less likely, SDL3)");
            }
        
            RemoveInputDevice(existingGamepad);
        }
    }
    
    private void HandleMouseButtonEvent(in SDL.Event @event)
    {
        var gpId = @event.Button.Which;
        var m = _mice.GetValueOrDefault(gpId);
        m?.ProcessButtonEvent(in @event.Button);
    }
    
    private void HandleMouseMotionEvent(in SDL.Event @event)
    {
        var gpId = @event.Motion.Which;
        var m = _mice.GetValueOrDefault(gpId);
        m?.ProcessMotionEvent(in @event.Motion);
    }
    
    private void HandleMouseWheelEvent(in SDL.Event @event)
    {
        var gpId = @event.Wheel.Which;
        var m = _mice.GetValueOrDefault(gpId);
        m?.ProcessWheelEvent(in @event.Wheel);
    }
    
    #endregion
    
    #region Input Device Lists

    private void AddInputDevice(SdlBaseInputDevice device)
    {
        _devices.Add(device);
        switch (device)
        {
            case SdlGamepad gp:
                _gamepads.Add(gp.InstanceId, gp);
                break;
            case SdlMouse m:
                _mice.Add(m.InstanceId, m);
                break;
            case SdlKeyboard k:
                _keyboards.Add(k.InstanceId, k);
                break;
        }
        
        _removedDevices.Remove(device);
        DeviceAdded?.Invoke(device);
    }

    private void RemoveInputDevice(SdlBaseInputDevice device)
    {
        _devices.Remove(device);
        switch (device)
        {
            case SdlGamepad gp:
                _gamepads.Remove(gp.InstanceId);
                break;
            case SdlMouse m:
                _mice.Remove(m.InstanceId);
                break;
            case SdlKeyboard k:
                _keyboards.Remove(k.InstanceId);
                break;
        }
        
        _removedDevices.Add(device);
        DeviceRemoved?.Invoke(device);
    }
    
    #endregion
    
    #region Text Input

    /// <inheritdoc />
    public void BeginTextInput(TextInputType type, TextInputCapitalization capitalization, TextInputFlags flags)
    {
        var props = SDL.CreateProperties();
        var inputType = type switch
        {
            TextInputType.Text => SDL.TextInputType.Text,
            TextInputType.Name => SDL.TextInputType.TextName,
            TextInputType.Email => SDL.TextInputType.TextEmail,
            TextInputType.Username => SDL.TextInputType.TextUsername,
            TextInputType.Password => SDL.TextInputType.NumberPasswordHidden,
            TextInputType.Number => SDL.TextInputType.Number,
            TextInputType.Pin => SDL.TextInputType.NumberPasswordHidden,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var capsMode = capitalization switch
        {
            TextInputCapitalization.None => SDL.Capitalization.None,
            TextInputCapitalization.Sentences => SDL.Capitalization.Sentences,
            TextInputCapitalization.Words => SDL.Capitalization.Words,
            TextInputCapitalization.Letters => SDL.Capitalization.Letters,
            _ => throw new ArgumentOutOfRangeException(nameof(capitalization), capitalization, null)
        };

        SDL.SetNumberProperty(props, SDL.Props.TextInputTypeNumber, (long)inputType);
        SDL.SetNumberProperty(props, SDL.Props.TextInputCapitalizationNumber, (long)capsMode);
        SDL.SetBooleanProperty(props, SDL.Props.TextInputAutoCorrectBoolean,
            flags.HasFlag(TextInputFlags.EnableAutoCorrect));
        SDL.SetBooleanProperty(props, SDL.Props.TextInputMultilineBoolean,
            flags.HasFlag(TextInputFlags.MultilineTextInput));

        SDL.StartTextInputWithProperties(_owner.NativeHandle, props);
        SDL.DestroyProperties(props);
    }

    /// <inheritdoc/>
    public void EndTextInput()
    {
        SDL.StopTextInput(_owner.NativeHandle);
    }

    /// <inheritdoc />
    public unsafe void SetTextInputArea(Rectangle? rect, int cursorOffset)
    {
        var r = new SDL.Rect();
        if (rect.HasValue)
        {
            r.X = rect.Value.X;
            r.Y = rect.Value.Y;
            r.W = rect.Value.Width;
            r.H = rect.Value.Height;
        }
        
        SDL.SetTextInputArea(_owner.NativeHandle, rect.HasValue ? (IntPtr)(&r) : IntPtr.Zero, cursorOffset);
    }

    #endregion
    
    #region Disposal

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _owner.EventProcess -= OnEvent;
        
        // Clear em all out
        _devices.Clear();
        _removedDevices.Clear();
        _gamepads.Clear();
        _mice.Clear();
        _keyboards.Clear();
    }
    
    #endregion
}