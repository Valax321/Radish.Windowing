using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// A keyboard input device.
/// </summary>
[PublicAPI]
public interface IKeyboard : IInputDevice
{
    /// <summary>
    /// Returns whether the given scancode is currently pressed.
    /// </summary>
    /// <remarks>This function accepts SCAN CODES, not KEY CODES. Use <see cref="KeycodeToScancode"/> to convert if needed.</remarks>
    /// <param name="scancode">The scancode code to check.</param>
    /// <returns><see langword="true"/> if the scancode is currently pressed, otherwise <see langword="false"/>.</returns>
    public bool IsPressed(Keys scancode);

    /// <summary>
    /// Converts the given key code (keyboard layout-independent) into its physical position on the keyboard.
    /// </summary>
    /// <seealso cref="ScancodeToKeycode"/>
    /// <example>If this method is passed <see cref="Keys.W"/>, a qwerty keyboard will return <see cref="Keys.W "/> but a dvorak keyboard would return <see cref="Keys.Oemcomma"/>.</example>
    /// <param name="keycode">The keycode to convert.</param>
    /// <returns>The converted scancode.</returns>
    public Keys KeycodeToScancode(Keys keycode);
    
    /// <summary>
    /// Converts the given scancode (keyboard layout-dependent) into its layout-independent key code.
    /// </summary>
    /// <seealso cref="KeycodeToScancode"/>
    /// <param name="keycode">The scancode to convert.</param>
    /// <returns>The converted scancode.</returns>
    public Keys ScancodeToKeycode(Keys keycode);
}