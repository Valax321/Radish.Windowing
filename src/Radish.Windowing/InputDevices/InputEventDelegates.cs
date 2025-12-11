namespace Radish.Windowing.InputDevices;

/// <summary>
/// Delegate invoked by <see cref="IInputContext"/> when text is entered into the application.
/// </summary>
public delegate void TextInputDelegate(ReadOnlySpan<char> text);

/// <summary>
/// Delegate invoked by <seealso cref="IInputContext"/> when an input device is added or removed.
/// </summary>
public delegate void InputDeviceChangeDelegate(IInputDevice device);

/// <summary>
/// Delegate invoked by <see cref="IGamepad"/> when a gamepad's layout changes.
/// </summary>
public delegate void GamepadRemappedDelegate();

/// <summary>
/// Delegate invoked by <see cref="IGamepad"/> when a button state changes.
/// </summary>
public delegate void GamepadButtonStateDelegate(GamepadButtons button, bool isDown);

/// <summary>
/// Delegate invoked by <see cref="IKeyboard"/> when a key state changes.
/// </summary>
public delegate void KeyboardKeyStateDelegate(Keys key, Scancodes scancode, bool isDown);

/// <summary>
/// Delegate invoked by <see cref="IMouse"/> when a mouse button state changes.
/// </summary>
public delegate void MouseButtonStateDelegate(MouseButtons button, bool isDown);
