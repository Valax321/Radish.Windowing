using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Axes on a standard gamepad.
/// </summary>
[PublicAPI]
public enum GamepadAxes
{
    /// <summary>
    /// Unknown/no axis.
    /// </summary>
    None = 0,
    /// <summary>
    /// The L2/Left trigger axis.
    /// </summary>
    /// <remarks>This may be not be an analog input on some platforms, but we still treat it as an axis for consistency.</remarks>
    LeftTrigger,
    /// <summary>
    /// The R2/Right trigger axis.
    /// </summary>
    /// <remarks>This may be not be an analog input on some platforms, but we still treat it as an axis for consistency.</remarks>
    RightTrigger,
    /// <summary>
    /// The left stick's X axis.
    /// </summary>
    LeftStickX,
    /// <summary>
    /// The left stick's Y axis.
    /// </summary>
    LeftStickY,
    /// <summary>
    /// The right stick's X axis.
    /// </summary>
    RightStickX,
    /// <summary>
    /// The right stick's Y axis.
    /// </summary>
    RightStickY,
}