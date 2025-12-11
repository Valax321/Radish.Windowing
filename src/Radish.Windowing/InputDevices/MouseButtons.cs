using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Mouse buttons that may be provided by a <see cref="IMouse"/> device.
/// </summary>
[PublicAPI]
public enum MouseButtons
{
    /// <summary>
    /// Unknown/no mouse button.
    /// </summary>
    None = 0,
    /// <summary>
    /// The primary (usually left) mouse button.
    /// </summary>
    Mouse1,
    /// <summary>
    /// The secondary (usually right) mouse button.
    /// </summary>
    Mouse2,
    /// <summary>
    /// Usually the middle mouse button (scroll wheel).
    /// </summary>
    Mouse3,
    /// <summary>
    /// Extra button, not available on all mice.
    /// </summary>
    Mouse4,
    /// <summary>
    /// Extra button, not available on all mice.
    /// </summary>
    Mouse5
}