using Radish.Windowing.InputDevices;

namespace Radish.Windowing;

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
