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
    
    /// <inheritdoc />
    public IMouse? PrimaryMouse => _mice.GetValueOrDefault(0u);

    /// <inheritdoc />
    public bool RelativeMouseMode
    {
        get
        {
            _owner.ThrowIfNativeHandleInvalid();
            return SDL.GetWindowRelativeMouseMode(_owner.NativeHandle);
        }
        set
        {
            _owner.ThrowIfNativeHandleInvalid();
            if (!SDL.SetWindowRelativeMouseMode(_owner.NativeHandle, value))
                throw new NativeWindowException(SDL.GetError());
        }
    }

    /// <inheritdoc />
    public bool TextInputActive => SDL.TextInputActive(_owner.NativeHandle);

    #endregion

    #region Private Fields
    
    private readonly Sdl3Window _owner;
    
    private readonly List<SdlBaseInputDevice> _devices = [];
    
    private readonly Dictionary<uint, SdlGamepad> _gamepads = [];
    private readonly Dictionary<uint, SdlKeyboard> _keyboards = [];
    private readonly Dictionary<uint, SdlMouse> _mice = [];
    
    #endregion

    #region Constructor
    
    internal Sdl3InputContext(Sdl3Window owner)
    {
        _owner = owner;
        _owner.EventProcess += OnEvent;
        _owner.PreEventProcess += ClearStaleMouseData;
        _owner.PostEventProcess += UpdateMouseDelta;
        
        HandleBuggedInitialEventReporting();
    }

    #endregion
    
    #region SDL Event Handling

    private void HandleBuggedInitialEventReporting()
    {
        // Basically, older SDL versions don't emit MouseAdded
        // events on startup like it does for gamepad events.
        // We can easily work around this by manually posting these events with the default devices,
        // but we have to be sure we're only doing it on the SDL versions with this issue.
        var v = VersionUtility.ParseSdlVersion(SDL.GetVersion());
        if (v.CompareTo(new Version(3, 3, 0)) >= 0) 
            return;
        
        var m = SDL.GetMice(out _);
        if (m != null)
        {
            foreach (var mid in m)
            {
                var ev = new SDL.Event
                {
                    Type = (uint)SDL.EventType.MouseAdded,
                    MDevice =
                    {
                        Which = mid,
                        Timestamp = SDL.GetTicksNS()
                    }
                };

                SDL.PushEvent(ref ev);
            }
        }
    }
    
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
                HandleGamepadButtonEvent(in @event); // BUG?? on mac at least this event never occurs
                break;
            case SDL.EventType.GamepadAxisMotion:
                HandleGamepadAxisEvent(in @event);
                break;
            case SDL.EventType.GamepadTouchpadDown:
            case SDL.EventType.GamepadTouchpadUp:
            case SDL.EventType.GamepadTouchpadMotion:
                HandleGamepadTouchEvent(in @event);
                break;
            case SDL.EventType.GamepadRemapped:
                HandleRemappedEvent(in @event);
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
                HandleMouseWheelEvent(in @event);
                break;
            
            case SDL.EventType.FingerDown:
                break;
            case SDL.EventType.FingerUp:
                break;
            case SDL.EventType.FingerMotion:
                break;
            case SDL.EventType.FingerCanceled:
                break;
        }
    }
    
    private void ClearStaleMouseData()
    {
        foreach (var (_, m) in _mice)
            m.ClearWheel();
    }
    
    private void UpdateMouseDelta()
    {
        foreach (var (_, m) in _mice)
            m.SampleDelta();
    }

    private void HandleKeyboardKeyEvent(in SDL.Event @event)
    {
        var kbId = @event.Key.Which;
        
        // Keyboards get extra special handling here because random shit can get picked up as a keyboard.
        // Don't actually register it as a keyboard until it sends some kind of input.
        if (!_keyboards.TryGetValue(kbId, out var kb))
        {
            kb = new SdlKeyboard(kbId);
            AddInputDevice(kb);
        }
        kb.ProcessKeyEvent(in @event.Key);
    }
    
    private void HandleKeyboardConnectionEvent(in SDL.Event @event)
    {
        var kbId = @event.KDevice.Which;

        if (@event.KDevice.Type == SDL.EventType.KeyboardAdded)
        {
            if (kbId != 0)
                return; // See comment in HandleKeyboardKeyEvent for why this exists.
            
            var existingKeyboard = new SdlKeyboard(kbId);
            AddInputDevice(existingKeyboard);
        }
        else if (@event.KDevice.Type == SDL.EventType.KeyboardRemoved)
        {
            if (!_keyboards.TryGetValue(kbId, out var existingKeyboard))
            {
                // If this ever happens it means we didn't catch a keyboard being inserted, which is a failure on our part.
                throw new InvalidOperationException(
                    "No existing keyboard instance in lookup table, this is a bug in Radish.Windowing (or less likely, SDL3)");
            }
        
            existingKeyboard.ClearEvents();
            RemoveInputDevice(existingKeyboard);
        }
    }
    
    private void HandleGamepadConnectionEvent(in SDL.Event @event)
    {
        var gpId = @event.GDevice.Which;
        if (@event.GDevice.Type == SDL.EventType.GamepadAdded)
        {
            // Gamepads need to be explicitly opened and closed, unlike other input devices.
            var ptr = SDL.OpenGamepad(gpId);
            if (ptr != IntPtr.Zero)
            {
                gpId = SDL.GetGamepadID(ptr);
            }
            
            var existingGamepad = new SdlGamepad(gpId);
            AddInputDevice(existingGamepad);
        }
        else if (@event.GDevice.Type == SDL.EventType.GamepadRemoved)
        {
            // Gamepads need to be explicitly opened and closed, unlike other input devices.
            var ptr = SDL.GetGamepadFromID(gpId);
            if (ptr != IntPtr.Zero)
            {
                SDL.CloseGamepad(ptr);
            }
            
            if (!_gamepads.TryGetValue(gpId, out var existingGamepad))
            {
                // If this ever happens it means we didn't catch a gamepad being inserted, which is a failure on our part.
                throw new InvalidOperationException(
                    "No existing gamepad instance in lookup table, this is a bug in Radish.Windowing (or less likely, SDL3)");
            }
        
            existingGamepad.ClearEvents();
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
        if (gpId == SDL.TouchMouseID)
            return;
        
        if (@event.MDevice.Type == SDL.EventType.MouseAdded)
        {
            if (gpId != 0) // Only add non-0 mice when they're used later
                return;
            
            var existingMouse = new SdlMouse(gpId);
            AddInputDevice(existingMouse);
        }
        else if (@event.MDevice.Type == SDL.EventType.MouseRemoved)
        {
            if (!_mice.TryGetValue(gpId, out var existingMouse))
            {
                // If this ever happens it means we didn't catch a mouse being inserted, which is a failure on our part.
                throw new InvalidOperationException(
                    "No existing keyboard instance in lookup table, this is a bug in Radish.Windowing (or less likely, SDL3)");
            }
        
            existingMouse.ClearEvents();
            RemoveInputDevice(existingMouse);
        }
    }
    
    private void HandleMouseButtonEvent(in SDL.Event @event)
    {
        var gpId = @event.Button.Which;
        if (gpId == SDL.TouchMouseID)
            return;
        
        // Like with keyboards, we only add mice once they're used.
        if (!_mice.TryGetValue(gpId, out var m))
        {
            m = new SdlMouse(gpId);
            AddInputDevice(m);
        }
        
        m.ProcessButtonEvent(in @event.Button);
    }
    
    private void HandleMouseMotionEvent(in SDL.Event @event)
    {
        var gpId = @event.Motion.Which;
        if (gpId == SDL.TouchMouseID)
            return;
        
        // Like with keyboards, we only add mice once they're used.
        if (!_mice.TryGetValue(gpId, out var m))
        {
            m = new SdlMouse(gpId);
            AddInputDevice(m);
        }
        
        m.ProcessMotionEvent(in @event.Motion);
    }
    
    private void HandleMouseWheelEvent(in SDL.Event @event)
    {
        var gpId = @event.Wheel.Which;
        if (gpId == SDL.TouchMouseID)
            return;
        
        // Like with keyboards, we only add mice once they're used.
        if (!_mice.TryGetValue(gpId, out var m))
        {
            m = new SdlMouse(gpId);
            AddInputDevice(m);
        }
        
        m.ProcessWheelEvent(in @event.Wheel);
    }
    
    #endregion
    
    #region Input Device Lists

    private void AddInputDevice(SdlBaseInputDevice device)
    {
        if (_devices.Contains(device))
            return;
        
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
        
        DeviceAdded?.Invoke(device);
    }

    private void RemoveInputDevice(SdlBaseInputDevice device)
    {
        if (!_devices.Contains(device))
            return;
        
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
        
        DeviceRemoved?.Invoke(device);
    }
    
    #endregion
    
    #region Text Input

    /// <inheritdoc />
    public IGamepad? GetGamepadByPlayerIndex(int index)
    {
        var gp = SDL.GetGamepadFromPlayerIndex(index);
        if (gp == IntPtr.Zero)
            return null;

        var id = SDL.GetGamepadID(gp);
        if (id == 0)
            return null;

        return _gamepads.GetValueOrDefault(id);
    }

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
        _owner.PreEventProcess -= ClearStaleMouseData;
        _owner.PostEventProcess -= UpdateMouseDelta;
        
        // Clear em all out
        _devices.Clear();
        _gamepads.Clear();
        _mice.Clear();
        _keyboards.Clear();
    }
    
    #endregion
}