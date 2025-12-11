using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// Arguments used in <see cref="WindowCloseRequestedDelegate"/>.
/// </summary>
[PublicAPI]
public struct WindowClosingEventArgs
{
    /// <summary>
    /// If set to false the window close request will be aborted.
    /// </summary>
    public bool ShouldClose;
}