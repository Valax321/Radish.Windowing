using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Interface for input devices provided by a <see cref="IInputContext"/>.
/// </summary>
[PublicAPI]
public interface IInputDevice
{
    /// <summary>
    /// Backend-specific native handle for this input device.
    /// </summary>
    public IntPtr NativeHandle { get; }
}