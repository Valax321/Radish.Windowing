using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Interface for input devices provided by a <see cref="IInputContext"/>.
/// </summary>
/// <remarks>
/// You shouldn't hold onto references to these objects, as they may be removed by the user at any time.
/// If a device is removed, and you still access it, we don't promise the program won't crash.
/// </remarks>
[PublicAPI]
public interface IInputDevice
{
    /// <summary>
    /// Backend-specific native handle for this input device.
    /// </summary>
    public IntPtr NativeHandle { get; }
}