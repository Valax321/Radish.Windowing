using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Known gamepad configurations that a <see cref="IGamepad"/> may match.
/// </summary>
[PublicAPI]
public enum GamepadModel
{
    /// <summary>
    /// An unknown type of gamepad.
    /// </summary>
    Generic,
    /// <summary>
    /// Gamepad is like a 360 controller.
    /// </summary>
    Xbox360,
    /// <summary>
    /// Gamepad is like an Xbox One controller.
    /// </summary>
    XboxOne,
    /// <summary>
    /// Gamepad is like a PS3 controller.
    /// </summary>
    Dualshock3,
    /// <summary>
    /// Gamepad is like a PS4 controller.
    /// </summary>
    Dualshock4,
    /// <summary>
    /// Gamepad is like a PS5 controller.
    /// </summary>
    Dualsense,
    /// <summary>
    /// Gamepad is like a Nintendo Switch Pro controller.
    /// </summary>
    SwitchPro,
    /// <summary>
    /// Gamepad is like a left Nintendo Switch Joycon.
    /// </summary>
    SwitchJoyconLeft,
    /// <summary>
    /// Gamepad is like a right Nintendo Switch Joycon.
    /// </summary>
    SwitchJoyconRight,
    /// <summary>
    /// Gamepad is like a pair of Nintendo Switch Joycons.
    /// </summary>
    SwitchJoyconPair
}