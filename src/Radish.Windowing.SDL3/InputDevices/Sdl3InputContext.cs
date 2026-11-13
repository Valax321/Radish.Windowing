using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
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

    /// <inheritdoc/>
    public IKeyboard? PrimaryKeyboard => _keyboards.GetValueOrDefault(0u);

    /// <inheritdoc />
    public bool RelativeMouseMode
    {
        get
        {
            _owner.ThrowIfNativeHandleInvalid();
            return SDL_GetWindowRelativeMouseMode(_owner.NativeHandle);
        }
        set
        {
            _owner.ThrowIfNativeHandleInvalid();
            if (!SDL_SetWindowRelativeMouseMode(_owner.NativeHandle, value))
                throw new NativeWindowException(SDL_GetError());
        }
    }

    /// <inheritdoc />
    public bool TextInputActive => SDL_TextInputActive(_owner.NativeHandle);

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

    private unsafe void HandleBuggedInitialEventReporting()
    {
        // Basically, older SDL versions don't emit MouseAdded
        // events on startup like it does for gamepad events.
        // We can easily work around this by manually posting these events with the default devices,
        // but we have to be sure we're only doing it on the SDL versions with this issue.
        var v = VersionUtility.ParseSdlVersion(SDL_GetVersion());
        if (v.CompareTo(new Version(3, 3, 0)) >= 0) 
            return;
        
        var m = (uint*)SDL_GetMice(out var cnt);
        try
        {
            if (m != null)
            {
                for (var i = 0; i < cnt; ++i)
                {
                    var mid = m[i];
                    var ev = new SDL_Event
                    {
                        type = (uint)SDL_EventType.SDL_EVENT_MOUSE_ADDED,
                        mdevice =
                        {
                            which = mid,
                            timestamp = SDL_GetTicksNS()
                        }
                    };

                    SDL_PushEvent(ref ev);
                }
            }
        }
        finally
        {
            SDL_free((nint)m);
        }
    }
    
    private unsafe void OnEvent(in SDL_Event @event)
    {
        switch ((SDL_EventType)@event.type)
        {
            // Input device add/removed
            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
                HandleGamepadConnectionEvent(in @event);
                break;
            case SDL_EventType.SDL_EVENT_KEYBOARD_ADDED:
            case SDL_EventType.SDL_EVENT_KEYBOARD_REMOVED:
                HandleKeyboardConnectionEvent(in @event);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_ADDED:
            case SDL_EventType.SDL_EVENT_MOUSE_REMOVED:
                HandleMouseConnectionEvent(in @event);
                break;
            
            // Text input
            case SDL_EventType.SDL_EVENT_TEXT_INPUT:
                var t = Utf8StringMarshaller.ConvertToManaged(@event.text.text);
                TextInput?.Invoke(t ?? ReadOnlySpan<char>.Empty);
                break;
            
            // keyboard input
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
            case SDL_EventType.SDL_EVENT_KEY_UP:
                HandleKeyboardKeyEvent(in @event);
                break;
            
            // gamepad input
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
                HandleGamepadButtonEvent(in @event); // BUG?? on mac at least this event never occurs
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
                HandleGamepadAxisEvent(in @event);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_DOWN:
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_UP:
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_MOTION:
                HandleGamepadTouchEvent(in @event);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMAPPED:
                HandleRemappedEvent(in @event);
                break;
            
            // mouse input
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
                HandleMouseButtonEvent(in @event);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                HandleMouseMotionEvent(in @event);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
                HandleMouseWheelEvent(in @event);
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

    private void HandleKeyboardKeyEvent(in SDL_Event @event)
    {
        var kbId = @event.key.which;
        
        // Keyboards get extra special handling here because random shit can get picked up as a keyboard.
        // Don't actually register it as a keyboard until it sends some kind of input.
        if (!_keyboards.TryGetValue(kbId, out var kb))
        {
            kb = new SdlKeyboard(kbId);
            AddInputDevice(kb);
        }
        kb.ProcessKeyEvent(in @event.key);
    }
    
    private void HandleKeyboardConnectionEvent(in SDL_Event @event)
    {
        var kbId = @event.kdevice.which;

        if (@event.kdevice.type == SDL_EventType.SDL_EVENT_KEYBOARD_ADDED)
        {
            if (kbId != 0)
                return; // See comment in HandleKeyboardKeyEvent for why this exists.
            
            var existingKeyboard = new SdlKeyboard(kbId);
            AddInputDevice(existingKeyboard);
        }
        else if (@event.kdevice.type == SDL_EventType.SDL_EVENT_KEYBOARD_REMOVED)
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
    
    private void HandleGamepadConnectionEvent(in SDL_Event @event)
    {
        var gpId = @event.gdevice.which;
        if (@event.gdevice.type == SDL_EventType.SDL_EVENT_GAMEPAD_ADDED)
        {
            // Gamepads need to be explicitly opened and closed, unlike other input devices.
            var ptr = SDL_OpenGamepad(gpId);
            if (ptr != IntPtr.Zero)
            {
                gpId = SDL_GetGamepadID(ptr);
            }
            
            var existingGamepad = new SdlGamepad(gpId);
            AddInputDevice(existingGamepad);
        }
        else if (@event.gdevice.type == SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED)
        {
            // Gamepads need to be explicitly opened and closed, unlike other input devices.
            var ptr = SDL_GetGamepadFromID(gpId);
            if (ptr != IntPtr.Zero)
            {
                SDL_CloseGamepad(ptr);
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
    
    private void HandleGamepadButtonEvent(in SDL_Event @event)
    {
        var gpId = @event.gbutton.which;
        var gp = _gamepads.GetValueOrDefault(gpId);
        gp?.ProcessButtonEvent(in @event.gbutton);
    }
    
    private void HandleGamepadAxisEvent(in SDL_Event @event)
    {
        var gpId = @event.gaxis.which;
        var gp = _gamepads.GetValueOrDefault(gpId);
        gp?.ProcessAxisEvent(in @event.gaxis);
    }
    
    private void HandleGamepadTouchEvent(in SDL_Event @event)
    {
        var gpId = @event.gtouchpad.which;
        var gp = _gamepads.GetValueOrDefault(gpId);
        gp?.ProcessTouchpadEvent(in @event.gtouchpad);
    }
    
    private void HandleRemappedEvent(in SDL_Event @event)
    {
        var gpId = @event.gdevice.which;
        var gp = _gamepads.GetValueOrDefault(gpId);
        gp?.ProcessRemapEvent();
    }
    
    private void HandleMouseConnectionEvent(in SDL_Event @event)
    {
        var gpId = @event.mdevice.which;
        
        if (@event.mdevice.type == SDL_EventType.SDL_EVENT_MOUSE_ADDED)
        {
            if (gpId != 0) // Only add non-0 mice when they're used later
                return;

            var existingMouse = new SdlMouse(gpId);
            AddInputDevice(existingMouse);
        }
        else if (@event.mdevice.type == SDL_EventType.SDL_EVENT_MOUSE_REMOVED)
        {
            if (!_mice.TryGetValue(gpId, out var existingMouse))
            {
                // If this ever happens it means we didn't catch a mouse being inserted, which is a failure on our part.
                throw new InvalidOperationException(
                    "No existing mouse instance in lookup table, this is a bug in Radish.Windowing (or less likely, SDL3)");
            }
        
            existingMouse.ClearEvents();
            RemoveInputDevice(existingMouse);
        }
    }
    
    private void HandleMouseButtonEvent(in SDL_Event @event)
    {
        var gpId = @event.button.which;

        // Like with keyboards, we only add mice once they're used.
        if (!_mice.TryGetValue(gpId, out var m))
        {
            m = new SdlMouse(gpId);
            AddInputDevice(m);
        }
        
        m.ProcessButtonEvent(in @event.button);
    }
    
    private void HandleMouseMotionEvent(in SDL_Event @event)
    {
        var gpId = @event.motion.which;

        // Like with keyboards, we only add mice once they're used.
        if (!_mice.TryGetValue(gpId, out var m))
        {
            m = new SdlMouse(gpId);
            AddInputDevice(m);
        }
        
        m.ProcessMotionEvent(in @event.motion);
    }
    
    private void HandleMouseWheelEvent(in SDL_Event @event)
    {
        var gpId = @event.wheel.which;

        // Like with keyboards, we only add mice once they're used.
        if (!_mice.TryGetValue(gpId, out var m))
        {
            m = new SdlMouse(gpId);
            AddInputDevice(m);
        }
        
        m.ProcessWheelEvent(in @event.wheel);
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
        var gp = SDL_GetGamepadFromPlayerIndex(index);
        if (gp == IntPtr.Zero)
            return null;

        var id = SDL_GetGamepadID(gp);
        if (id == 0)
            return null;

        return _gamepads.GetValueOrDefault(id);
    }

    /// <inheritdoc />
    public void BeginTextInput(TextInputType type, TextInputCapitalization capitalization, TextInputFlags flags)
    {
        var props = SDL_CreateProperties();
        var inputType = type switch
        {
            TextInputType.Text => SDL_TextInputType.SDL_TEXTINPUT_TYPE_TEXT,
            TextInputType.Name => SDL_TextInputType.SDL_TEXTINPUT_TYPE_TEXT_NAME,
            TextInputType.Email => SDL_TextInputType.SDL_TEXTINPUT_TYPE_TEXT_EMAIL,
            TextInputType.Username => SDL_TextInputType.SDL_TEXTINPUT_TYPE_TEXT_USERNAME,
            TextInputType.Password => SDL_TextInputType.SDL_TEXTINPUT_TYPE_TEXT_PASSWORD_HIDDEN,
            TextInputType.Number => SDL_TextInputType.SDL_TEXTINPUT_TYPE_NUMBER,
            TextInputType.Pin => SDL_TextInputType.SDL_TEXTINPUT_TYPE_NUMBER_PASSWORD_HIDDEN,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var capsMode = capitalization switch
        {
            TextInputCapitalization.None => SDL_Capitalization.SDL_CAPITALIZE_NONE,
            TextInputCapitalization.Sentences => SDL_Capitalization.SDL_CAPITALIZE_SENTENCES,
            TextInputCapitalization.Words => SDL_Capitalization.SDL_CAPITALIZE_WORDS,
            TextInputCapitalization.Letters => SDL_Capitalization.SDL_CAPITALIZE_LETTERS,
            _ => throw new ArgumentOutOfRangeException(nameof(capitalization), capitalization, null)
        };

        SDL_SetNumberProperty(props, SDL_PROP_TEXTINPUT_TYPE_NUMBER, (long)inputType);
        SDL_SetNumberProperty(props, SDL_PROP_TEXTINPUT_CAPITALIZATION_NUMBER, (long)capsMode);
        SDL_SetBooleanProperty(props, SDL_PROP_TEXTINPUT_AUTOCORRECT_BOOLEAN,
            flags.HasFlag(TextInputFlags.EnableAutoCorrect));
        SDL_SetBooleanProperty(props, SDL_PROP_TEXTINPUT_MULTILINE_BOOLEAN,
            flags.HasFlag(TextInputFlags.MultilineTextInput));

        SDL_StartTextInputWithProperties(_owner.NativeHandle, props);
        SDL_DestroyProperties(props);
    }

    /// <inheritdoc/>
    public void EndTextInput()
    {
        SDL_StopTextInput(_owner.NativeHandle);
    }

    /// <inheritdoc />
    public unsafe void SetTextInputArea(Rectangle? rect, int cursorOffset)
    {
        if (rect.HasValue)
        {
            var r = new SDL_Rect
            {
                x = rect.Value.X,
                y = rect.Value.Y,
                w = rect.Value.Width,
                h = rect.Value.Height
            };

            SDL_SetTextInputArea(_owner.NativeHandle, ref r, cursorOffset);
        }
        else
        {
            SDL_SetTextInputArea(_owner.NativeHandle, ref Unsafe.NullRef<SDL_Rect>(), cursorOffset);
        }
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