using System.Drawing;
using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// Parameters passed to a window when it is created.
/// </summary>
/// <param name="Title">The title of the window. If null, the application name is used.</param>
/// <param name="Size">The initial size of the window.</param>
/// <param name="MinimumSize">The minimum size of the window. If set to null, the window has no minimum size.</param>
/// <param name="MaximumSize">The maximum size of the window. If set to null, the window has no maximum size.</param>
/// <param name="Resizable">If set to true the window can be resized by the user.</param>
/// <param name="Hidden">If set to true the window will be hidden until <see cref="IWindow.IsVisible"/> is set to true.</param>
/// <param name="BackendParameters">Backend-specific window data. Some backends may provide additional options via an implementation of <see cref="IBackendWindowParameters"/> passed here.</param>
[PublicAPI]
public record struct WindowInitParameters(
    Size Size,
    string? Title = null,
    Size? MinimumSize = null,
    Size? MaximumSize = null,
    bool Resizable = true,
    bool Hidden = false,
    IBackendWindowParameters? BackendParameters = null
);
