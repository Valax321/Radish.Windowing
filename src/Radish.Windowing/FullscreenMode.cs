using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// Fullscreen mode used by the window.
/// </summary>
[PublicAPI]
public enum FullscreenMode
{
    /// <summary>
    /// Non-fullscreen mode. Window can be resized by the user if <see cref="WindowInitParameters.Resizable"/> is set.
    /// </summary>
    Windowed,
    /// <summary>
    /// Desktop fullscreen mode. The resolution in this mode is always fixed to the desktop video mode.
    /// </summary>
    Desktop,
    /// <summary>
    /// Proper fullscreen mode. User-selectable resolution.
    /// </summary>
    Exclusive
}