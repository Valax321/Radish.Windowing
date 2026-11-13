using System.Collections;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.Utility;

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

    internal void ProcessKeyEvent(in SDL_KeyboardEvent kb)
    {
        var sc = (Scancodes)kb.scancode;
        // We don't know what kind of key this is, don't bother with it
        if (sc == Scancodes.None || !Enum.IsDefined(sc))
            return;
        
        _keyMask[KeyIndexLookup[sc]] = kb.down;
        
        if (kb.down)
            KeyDown?.Invoke(ScancodeToKeycode(sc), sc, true);
        else
            KeyUp?.Invoke(ScancodeToKeycode(sc), sc, false);
    }
    
    #region Keycode hell

    public Scancodes KeycodeToScancode(Keys keycode)
    {
        var sdlKey = (SDL_Keycode)keycode;
        if (sdlKey == SDL_Keycode.SDLK_UNKNOWN)
            return Scancodes.None;
        
        var scancode = SDL_GetScancodeFromKey((uint)sdlKey, 0);
        var sc2 = (Scancodes)scancode;
        return Enum.IsDefined(sc2) ? sc2 : Scancodes.None;
    }

    public Keys ScancodeToKeycode(Scancodes scancode)
    {
        var sdlScancode = (SDL_Scancode)scancode;
        if (sdlScancode == SDL_Scancode.SDL_SCANCODE_UNKNOWN)
            return Keys.None;
        
        var key = SDL_GetKeyFromScancode(sdlScancode, SDL_Keymod.SDL_KMOD_NONE, false);
        var k2 = (Keys)key;
        return Enum.IsDefined(k2) ? k2 : Keys.None;
    }
    
    #endregion

    public void ClearEvents()
    {
        KeyDown = null;
        KeyUp = null;
    }

    public override string ToString()
    {
        var s = SDL_GetKeyboardNameForID(InstanceId);
        return string.IsNullOrEmpty(s) ? "Unnamed Keyboard" : s;
    }
}