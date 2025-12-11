using System.Collections;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.Utility;
using SDL3;

namespace Radish.Windowing.SDL3.InputDevices;

internal sealed class SdlKeyboard : SdlBaseInputDevice, IKeyboard
{
    // Used to map keycode values to BitArray indices
    private static readonly Dictionary<Keys, int> KeyIndexLookup = EnumUtility.GetEnumIndexMap<Keys>();
    
    private readonly BitArray _keyMask = new(KeyIndexLookup.Count);
    
    public SdlKeyboard(uint instanceId)
    {
        InstanceId = instanceId;
    }

    public bool IsPressed(Keys scancode)
    {
        if (scancode == Keys.None)
            return false;
        
        return _keyMask[KeyIndexLookup[scancode]];
    }

    internal void ProcessKeyEvent(in SDL.KeyboardEvent kb)
    {
        var sc = SdlToRadishScancodes.GetValueOrDefault(kb.Scancode, Keys.None);
        // We don't know what kind of key this is, don't bother with it
        if (sc == Keys.None)
            return;
        
        _keyMask[KeyIndexLookup[sc]] = kb.Down;
    }
    
    #region Keycode hell

    public Keys KeycodeToScancode(Keys keycode)
    {
        var sdlKey = RadishToSdlKeys.GetValueOrDefault(keycode, SDL.Keycode.Unknown);
        if (sdlKey == SDL.Keycode.Unknown)
            return Keys.None;
        
        var scancode = SDL.GetScancodeFromKey(sdlKey, out _);
        return SdlToRadishScancodes.GetValueOrDefault(scancode, Keys.None);
    }

    public Keys ScancodeToKeycode(Keys scancode)
    {
        var sdlScancode = RadishToSdlScancodes.GetValueOrDefault(scancode, SDL.Scancode.Unknown);
        if (sdlScancode == SDL.Scancode.Unknown)
            return Keys.None;
        
        var key = SDL.GetKeyFromScancode(sdlScancode, SDL.Keymod.None, false);
        return SdlToRadishKeys.GetValueOrDefault(key, Keys.None);
    }
    
    private static readonly Dictionary<SDL.Keycode, Keys> SdlToRadishKeys = new()
    {
        //TODO
        { SDL.Keycode.A, Keys.A }
    };

    private static readonly Dictionary<Keys, SDL.Keycode> RadishToSdlKeys = new()
        //TODO
    {
        { Keys.A, SDL.Keycode.A }
    };

    private static readonly Dictionary<SDL.Scancode, Keys> SdlToRadishScancodes = new()
    {
        //TODO
        { SDL.Scancode.A, Keys.A }
    };

    private static readonly Dictionary<Keys, SDL.Scancode> RadishToSdlScancodes = new()
    {
        //TODO
        { Keys.A, SDL.Scancode.A }
    };
    
    #endregion
}