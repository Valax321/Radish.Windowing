using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// A keyboard input device.
/// </summary>
[PublicAPI]
public interface IKeyboard : IInputDevice
{
    /// <summary>
    /// Invoked when a key is pressed.
    /// </summary>
    public event KeyboardKeyStateDelegate KeyDown;
    
    /// <summary>
    /// Invoked when a key is released.
    /// </summary>
    public event KeyboardKeyStateDelegate KeyUp;
    
    /// <summary>
    /// Returns whether the given scancode is currently pressed.
    /// </summary>
    /// <remarks>This function accepts SCAN CODES, not KEY CODES. Use <see cref="KeycodeToScancode"/> to convert if needed.</remarks>
    /// <param name="scancode">The scancode code to check.</param>
    /// <returns><see langword="true"/> if the scancode is currently pressed, otherwise <see langword="false"/>.</returns>
    public bool IsPressed(Scancodes scancode);

    /// <summary>
    /// Converts the given key code (keyboard layout-independent) into its physical position on the keyboard.
    /// </summary>
    /// <seealso cref="ScancodeToKeycode"/>
    /// <param name="keycode">The keycode to convert.</param>
    /// <returns>The converted scancode.</returns>
    public Scancodes KeycodeToScancode(Keys keycode);
    
    /// <summary>
    /// Converts the given scancode (keyboard layout-dependent) into its layout-independent key code.
    /// </summary>
    /// <seealso cref="KeycodeToScancode"/>
    /// <param name="scancode">The scancode to convert.</param>
    /// <returns>The converted scancode.</returns>
    public Keys ScancodeToKeycode(Scancodes scancode);
}