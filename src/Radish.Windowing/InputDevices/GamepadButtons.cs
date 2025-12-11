using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Buttons present on a standard gamepad.
/// </summary>
[PublicAPI]
public enum GamepadButtons
{
    /// <summary>
    /// Unknown/no button.
    /// </summary>
    None = 0,
    /// <summary>
    /// The Cross/A button.
    /// </summary>
    FacePadSouth,
    /// <summary>
    /// The Triangle/Y button.
    /// </summary>
    FacePadNorth,
    /// <summary>
    /// The Circle/B button.
    /// </summary>
    FacePadEast,
    /// <summary>
    /// The Square/X button.
    /// </summary>
    FacePadWest,
    /// <summary>
    /// The DPad down button.
    /// </summary>
    DPadSouth,
    /// <summary>
    /// The DPad up button.
    /// </summary>
    DPadNorth,
    /// <summary>
    /// The DPad right button.
    /// </summary>
    DPadEast,
    /// <summary>
    /// The DPad left button.
    /// </summary>
    DPadWest,
    /// <summary>
    /// The L1/LT button.
    /// </summary>
    LeftShoulder,
    /// <summary>
    /// The R1/RT button.
    /// </summary>
    RightShoulder,
    /// <summary>
    /// The L3/Left stick click button.
    /// </summary>
    LeftStickPress,
    /// <summary>
    /// The R3/Right stick click button.
    /// </summary>
    RightStickPress,
    /// <summary>
    /// The touchpad button.
    /// </summary>
    /// <remarks>Only present on Dualshock 4 or Dualsense type controllers.</remarks>
    TouchpadPress,
    /// <summary>
    /// The Start/Menu button.
    /// </summary>
    Start,
    /// <summary>
    /// The Select/Back button. Not present or usable on some platforms.
    /// </summary>
    Select
}