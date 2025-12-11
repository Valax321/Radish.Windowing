using System.Collections;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.Utility;
using SDL3;

namespace Radish.Windowing.SDL3.InputDevices;

internal sealed class SdlKeyboard : SdlBaseInputDevice, IKeyboard
{
    // Used to map keycode values to BitArray indices
    private static readonly Dictionary<Scancodes, int> KeyIndexLookup = EnumUtility.GetEnumIndexMap<Scancodes>();
    
    private readonly BitArray _keyMask = new(KeyIndexLookup.Count);
    
    public SdlKeyboard(uint instanceId)
    {
        InstanceId = instanceId;
    }

    public event KeyboardKeyStateDelegate? KeyDown;
    public event KeyboardKeyStateDelegate? KeyUp;

    public bool IsPressed(Scancodes scancode)
    {
        if (scancode == Scancodes.None)
            return false;
        
        return _keyMask[KeyIndexLookup[scancode]];
    }

    internal void ProcessKeyEvent(in SDL.KeyboardEvent kb)
    {
        var sc = (Scancodes)kb.Scancode;
        // We don't know what kind of key this is, don't bother with it
        if (sc == Scancodes.None || !Enum.IsDefined(sc))
            return;
        
        _keyMask[KeyIndexLookup[sc]] = kb.Down;
        
        if (kb.Down)
            KeyDown?.Invoke(ScancodeToKeycode(sc), sc, true);
        else
            KeyUp?.Invoke(ScancodeToKeycode(sc), sc, false);
    }
    
    #region Keycode hell

    public Scancodes KeycodeToScancode(Keys keycode)
    {
        var sdlKey = (SDL.Keycode)keycode;
        if (sdlKey == SDL.Keycode.Unknown)
            return Scancodes.None;
        
        var scancode = SDL.GetScancodeFromKey(sdlKey, out _);
        var sc2 = (Scancodes)scancode;
        return Enum.IsDefined(sc2) ? sc2 : Scancodes.None;
    }

    public Keys ScancodeToKeycode(Scancodes scancode)
    {
        var sdlScancode = (SDL.Scancode)scancode;
        if (sdlScancode == SDL.Scancode.Unknown)
            return Keys.None;
        
        var key = SDL.GetKeyFromScancode(sdlScancode, SDL.Keymod.None, false);
        var k2 = (Keys)key;
        return Enum.IsDefined(k2) ? k2 : Keys.None;
    }
    
    #endregion

    public override void ClearEvents()
    {
        KeyDown = null;
        KeyUp = null;
    }

    public override string ToString()
    {
        var s = SDL.GetKeyboardNameForID(InstanceId);
        return string.IsNullOrEmpty(s) ? "Unnamed Keyboard" : s;
    }
}