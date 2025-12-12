using System.Drawing;
using JetBrains.Annotations;
using Radish.Windowing.InputDevices;

namespace Radish.Windowing;

/// <summary>
/// Interface to a window.
/// </summary>
[PublicAPI]
public interface IWindow : IDisposable
{
    /// <summary>
    /// Handle to the backend's native window pointer. Can be used for direct access.
    /// </summary>
    public IntPtr NativeHandle { get; }

    /// <summary>
    /// The string displayed on the window title bar.
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// If true the window is visible on the user's display.
    /// </summary>
    public bool IsVisible { get; set; }
    
    /// <summary>
    /// Current minimised/maximised state of the window.
    /// </summary>
    public WindowState WindowState { get; set; }
    
    /// <summary>
    /// Whether the user is allowed to resize the window.
    /// </summary>
    /// <remarks>The window may still be forcibly resized by the OS for any reason. This just hides/disables the user controls to do it.</remarks>
    public bool Resizable { get; set; }
    
    /// <summary>
    /// Is the window focused?
    /// </summary>
    public bool Focused { get; }
    
    /// <summary>
    /// The window's position on the desktop, in desktop coordinates.
    /// Setting the position is only supported in <see cref="Windowing.FullscreenMode.Windowed"/> mode.
    /// </summary>
    public Point Position { get; set; }
    
    /// <summary>
    /// The size of the window in desktop coordinates.
    /// </summary>
    /// <remarks>This is not the pixel size of the window, use <see cref="PixelSize"/> for that.</remarks>
    public Size Size { get; set; }
    
    /// <summary>
    /// The size of the window in pixels.
    /// </summary>
    public Size PixelSize { get; }
    
    /// <summary>
    /// The minimum size of the window in desktop coordinates.
    /// </summary>
    public Size MinimumSize { get; set; }
    
    /// <summary>
    /// The maximum size of the window in desktop coordinates.
    /// </summary>
    public Size MaximumSize { get; set; }
    
    /// <summary>
    /// The fullscreen mode for the window.
    /// </summary>
    public FullscreenMode FullscreenMode { get; set; }
    
    /// <summary>
    /// The desktop coord to pixels conversion factor.
    /// </summary>
    public float ContentScale { get; }
    
    /// <summary>
    /// Gets the current display showing the window.
    /// </summary>
    public IDisplay CurrentDisplay { get; set; }
    
    /// <summary>
    /// Gets the primary display of this system.
    /// </summary>
    public IDisplay PrimaryDisplay { get;}
    
    /// <summary>
    /// Gets the displays that can host this window.
    /// </summary>
    public IEnumerable<IDisplay> Displays { get; }

    /// <summary>
    /// Invoked when the window has finished initialising and its internal state can be safely accessed.
    /// </summary>
    public event WindowLoadDelegate Loaded;

    /// <summary>
    /// Invoked when the window is updating.
    /// </summary>
    public event WindowUpdateDelegate Update;

    /// <summary>
    /// Invoked when the window is rendering a frame.
    /// </summary>
    public event WindowRenderDelegate Render;

    /// <summary>
    /// Invoked when the window wants to close.
    /// </summary>
    public event WindowCloseRequestedDelegate CloseRequested;

    /// <summary>
    /// Invoked when the window is about to close.
    /// </summary>
    public event WindowClosingDelegate Closing;

    /// <summary>
    /// Invoked when the window is moved to a new position.
    /// </summary>
    public event WindowMovedDelegate Moved;

    /// <summary>
    /// Invoked when the window's size changes.
    /// </summary>
    public event WindowResizedDelegate Resized;

    /// <summary>
    /// Invoked when the window's pixel size changes.
    /// </summary>
    public event WindowPixelSizeChangedDelegate PixelSizeChanged;

    /// <summary>
    /// Invoked when the window gains focus.
    /// </summary>
    public event WindowFocusChangedDelegate GainedFocus;

    /// <summary>
    /// Invoked when the window loses focus.
    /// </summary>
    public event WindowFocusChangedDelegate LostFocus;

    /// <summary>
    /// Invoked when the window's display changes.
    /// </summary>
    public event WindowDisplayChangedDelegate DisplayChanged;

    /// <summary>
    /// Invoked when a new display is added to the desktop.
    /// </summary>
    public event DisplaysChangedDelegate DisplayAdded;

    /// <summary>
    /// Invoked when a display is removed from the desktop.
    /// </summary>
    public event DisplaysChangedDelegate DisplayRemoved;

    /// <summary>
    /// Creates an input context for the given window.
    /// </summary>
    /// <returns></returns>
    public IInputContext CreateInput();

    /// <summary>
    /// Runs the main loop for the window.
    /// Before this is called, none of the internal window state is valid and accessing properties can result in undefined behaviour.
    /// </summary>
    public void Run();

    /// <summary>
    /// Request to close the window.
    /// </summary>
    /// <param name="force">If <see langword="true"/> then the <see cref="CloseRequested"/> callback will be skipped.</param>
    public void Close(bool force = false);

    /// <summary>
    /// Provides a batch operation to move, resize and change the fullscreen mode of the window.
    /// Doing all these at once may be more efficient than setting each property individually.
    /// </summary>
    /// <param name="display">The display to move to.</param>
    /// <param name="mode">The fullscreen mode to switch to.</param>
    /// <param name="videoMode">The video mode to switch to.</param>
    public bool MoveToDisplay(IDisplay display, FullscreenMode mode, VideoMode videoMode);
}
