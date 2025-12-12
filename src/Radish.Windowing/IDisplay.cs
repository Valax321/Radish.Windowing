using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// A monitor/display the game window can be shown on.
/// </summary>
[PublicAPI]
public interface IDisplay : IEquatable<IDisplay>
{
    /// <summary>
    /// A numeric identifier for the display.
    /// </summary>
    public uint DisplayIndex { get; }
    
    /// <summary>
    /// The native video mode for this display.
    /// </summary>
    public VideoMode NativeVideoMode { get; }
    
    /// <summary>
    /// The current video mode for this display.
    /// </summary>
    public VideoMode CurrentVideoMode { get; }
    
    /// <summary>
    /// The supported full-screen video modes for this display.
    /// </summary>
    public IEnumerable<VideoMode> FullscreenVideoModes { get; }
    
    /// <summary>
    /// Gets the conversion factor from desktop coordinates to pixels.
    /// </summary>
    public float ContentScale { get; }
    
    /// <summary>
    /// Gets whether this window is detected to support HDR output.
    /// </summary>
    public bool SupportsHDR { get; }
    
    /// <summary>
    /// Native handle for the display.
    /// </summary>
    public IntPtr NativeHandle { get; }
}