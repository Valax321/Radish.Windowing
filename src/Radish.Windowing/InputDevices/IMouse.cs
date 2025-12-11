using System.Numerics;
using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// A mouse input device.
/// </summary>
[PublicAPI]
public interface IMouse : IInputDevice
{
    /// <summary>
    /// Invoked when a mouse button is pressed.
    /// </summary>
    public event MouseButtonStateDelegate ButtonDown;
    
    /// <summary>
    /// Invoked when a mouse button is release.d
    /// </summary>
    public event MouseButtonStateDelegate ButtonUp;
    
    /// <summary>
    /// Gets the current position of the mouse.
    /// </summary>
    public Vector2 Position { get; }
    
    /// <summary>
    /// The relative movement speed of the mouse.
    /// </summary>
    public Vector2 PositionDelta { get; }
    
    /// <summary>
    /// The mouse wheel axis values.
    /// </summary>
    public Vector2 WheelAxes { get; }
    
    /// <summary>
    /// Gets whether the given mouse button is currently pressed.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns><see langword="true"/> if pressed, otherwise <see langword="false"/>.</returns>
    public bool IsPressed(MouseButtons button);
}