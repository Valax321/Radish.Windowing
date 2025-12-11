using System.Drawing;
using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Provides access to the input system for a window.
/// </summary>
[PublicAPI]
public interface IInputContext : IDisposable
{
    /// <summary>
    /// Event invoked when text is entered by the user.
    /// This event is only called when <see cref="BeginTextInput"/> is called and is stopped when <see cref="EndTextInput"/> is called.
    /// <see cref="IKeyboard"/> should not be used for handling user text input.
    /// </summary>
    public event TextInputDelegate TextInput;
    
    /// <summary>
    /// Event invoked when an input device is added.
    /// </summary>
    public event InputDeviceChangeDelegate DeviceAdded;

    /// <summary>
    /// Event invoked when an input device is removed.
    /// </summary>
    public event InputDeviceChangeDelegate DeviceRemoved;
    
    /// <summary>
    /// A collection of all currently available input devices.
    /// </summary>
    public IReadOnlyCollection<IInputDevice> InputDevices { get; }
    
    /// <summary>
    /// A collection of all currently available keyboards.
    /// </summary>
    public IReadOnlyCollection<IKeyboard> Keyboards { get; }
    
    /// <summary>
    /// A collection of all currently available mice.
    /// </summary>
    public IReadOnlyCollection<IMouse> Mice { get; }
    
    /// <summary>
    /// A collection of all currently available gamepads.
    /// </summary>
    public IReadOnlyCollection<IGamepad> Gamepads { get; }

    /// <summary>
    /// Gets the gamepad with the given player ID, if one exists.
    /// </summary>
    /// <param name="index">The index to check.</param>
    /// <returns>The gamepad if found, otherwise <see langword="null"/>.</returns>
    public IGamepad? GetGamepadByPlayerIndex(int index);

    /// <summary>
    /// Begin receiving text input.
    /// </summary>
    public void BeginTextInput(TextInputType type = TextInputType.Text, TextInputCapitalization capitalization = TextInputCapitalization.None, TextInputFlags flags = TextInputFlags.EnableAutoCorrect);
    
    /// <summary>
    /// End receiving text input.
    /// </summary>
    public void EndTextInput();

    /// <summary>
    /// Sets or clears the text input area rectangle.
    /// This is used to hint to on-screen keyboards or IME systems where their own UI should draw.
    /// If <paramref name="rect"/> is <see langword="null"/>, the rectangle area is cleared.
    /// </summary>
    /// <param name="rect">The rectangle to set, or <see langword="null"/> to clear it.</param>
    /// <param name="cursorOffset">Offset of the text cursor relative to <paramref name="rect"/>'s <see cref="Rectangle.X"/> parameter. Value is unused if <paramref name="rect"/> is <see langword="null"/>.</param>
    public void SetTextInputArea(Rectangle? rect, int cursorOffset = 0);
}