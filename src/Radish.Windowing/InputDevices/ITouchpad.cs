using System.Numerics;
using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// A touchpad on a <see cref="IGamepad"/>.
/// </summary>
[PublicAPI]
public interface ITouchpad
{
    /// <summary>
    /// Data about a single finger on a touchpad.
    /// </summary>
    /// <param name="Position">The position of the finger. Probably normalised. If there is no touch this is NaN.</param>
    /// <param name="Pressure">The current pressure of the finger. 0-1 range. If there is no touch this is -1.</param>
    public readonly record struct FingerData(Vector2 Position, float Pressure)
    {
        /// <summary>
        /// <see langword="true"/> if there is a touch occurring, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsTouching => Pressure >= 0;
    }
    
    /// <summary>
    /// List of fingers on the touchpad. The length of this list matches the maximum number of supported touches on this touchpad.
    /// </summary>
    public IReadOnlyList<FingerData> Fingers { get; }
}