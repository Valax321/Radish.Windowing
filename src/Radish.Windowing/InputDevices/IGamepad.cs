using System.Numerics;
using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// A gamepad input device.
/// </summary>
[PublicAPI]
public interface IGamepad : IInputDevice
{
    /// <summary>
    /// Invoked when a gamepad's layout changes.
    /// </summary>
    public event GamepadRemappedDelegate Remapped;
    
    /// <summary>
    /// What model of gamepad this is. Value may not be highly reliable with third-party gamepads,
    /// but is good enough for deciding on icon sets for on-screen prompts, etc.
    /// </summary>
    public GamepadModel Model { get; }
    
    /// <summary>
    /// A list of touchpads this gamepad has.
    /// </summary>
    public IReadOnlyList<ITouchpad> Touchpads { get; }

    /// <summary>
    /// Gets whether the given gamepad button is currently pressed.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns><see langword="true"/> if pressed, otherwise <see langword="false"/>.</returns>
    public bool IsPressed(GamepadButtons button);

    /// <summary>
    /// Returns the current value of the given gamepad axis.
    /// </summary>
    /// <remarks>Triggers are represented as a normalised 0-1 range, while sticks use a -1 to 1 range.</remarks>
    /// <param name="axis">The axis to check.</param>
    /// <returns>A normalised representation of the axis value.</returns>
    public float GetAxisValue(GamepadAxes axis);
}