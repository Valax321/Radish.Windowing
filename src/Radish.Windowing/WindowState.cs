using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// Minimised/maximised state of a window.
/// </summary>
[PublicAPI]
public enum WindowState
{
    /// <summary>
    /// Not minimised or maximised.
    /// </summary>
    Normal,
    /// <summary>
    /// Window is minimised.
    /// </summary>
    Minimized,
    /// <summary>
    /// Window is maximised.
    /// </summary>
    Maximized,
}